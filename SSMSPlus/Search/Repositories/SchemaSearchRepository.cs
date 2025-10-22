using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSMSPlus.Core.Integration.Connection;
using SSMSPlus.Core.Utils;
using SSMSPlus.Search.Entities;

namespace SSMSPlus.Search.Repositories;

public class SchemaSearchRepository {
    private readonly object _lock = new object();
    private readonly Dictionary<int, DbDefinition> _databases = new Dictionary<int, DbDefinition>();
    private readonly Dictionary<int, DbObject[]> _dbObjects = new Dictionary<int, DbObject[]>();
    private readonly Dictionary<int, DbColumn[]> _dbColumns = new Dictionary<int, DbColumn[]>();
    private readonly Dictionary<int, DbIndex[]> _dbIndices = new Dictionary<int, DbIndex[]>();
    private readonly Dictionary<int, DbIndexColumn[]> _dbIndicesColumns = new Dictionary<int, DbIndexColumn[]>();
    private int _nextDbId = 1;

    public Task<int> DropDbAsync(int dbid) {
        lock (_lock) {
            _databases.Remove(dbid);
            _dbObjects.Remove(dbid);
            _dbColumns.Remove(dbid);
            _dbIndices.Remove(dbid);
            _dbIndicesColumns.Remove(dbid);
            return Task.FromResult(1);
        }
    }

    public Task<int> DbExistsAsync(DbConnectionString dbConnectionString) {
        lock (_lock) {
            var existingDb = _databases.Values
                .Where(db => string.Equals(db.Server, dbConnectionString.Server, StringComparison.InvariantCultureIgnoreCase) &&
                             string.Equals(db.DbName, dbConnectionString.Database, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(db => db.DbId)
                .FirstOrDefault();
            return Task.FromResult(existingDb?.DbId ?? 0);
        }
    }

    public Task<int> InsertDbAsync(DbDefinition dbdefinition, DbObject[] dbObjects, DbColumn[] columns, DbIndex[] indices, DbIndexColumn[] indicesColumns) {
        lock (_lock) {
            var dbId = _nextDbId++;
            dbdefinition.DbId = dbId;
            dbObjects.ForEach(p => p.DbId = dbId);
            columns.ForEach(p => p.DbId = dbId);
            indices.ForEach(p => p.DbId = dbId);
            indicesColumns.ForEach(p => p.DbId = dbId);
            _databases[dbId] = dbdefinition;
            _dbObjects[dbId] = dbObjects;
            _dbColumns[dbId] = columns;
            _dbIndices[dbId] = indices;
            _dbIndicesColumns[dbId] = indicesColumns;
            return Task.FromResult(dbId);
        }
    }

    public Task<ISearchTarget[]> GetObjectsByDbAsync(int dbid) {
        DbObject[] dbObjects;
        DbColumn[] dbColumns;
        DbIndex[] dbIndices;
        DbIndexColumn[] dbIndicesColumns;
        lock (_lock) {
            if (!_dbObjects.TryGetValue(dbid, out dbObjects)) dbObjects = Array.Empty<DbObject>();
            if (!_dbColumns.TryGetValue(dbid, out dbColumns)) dbColumns = Array.Empty<DbColumn>();
            if (!_dbIndices.TryGetValue(dbid, out dbIndices)) dbIndices = Array.Empty<DbIndex>();
            if (!_dbIndicesColumns.TryGetValue(dbid, out dbIndicesColumns)) dbIndicesColumns = Array.Empty<DbIndexColumn>();
        }

        var dbobjecstByID = dbObjects.ToDictionary(p => p.ObjectId);
        MapDbObjectParents(dbobjecstByID);
        MapdbColumnsParents(dbobjecstByID, dbColumns);
        MapdbIndicesParents(dbobjecstByID, dbIndices);
        MapIndicesColumns(dbIndices, dbIndicesColumns);

        var dbColumnsByTableId = dbColumns.ToLookup(p => p.TableId);
        var dbObjectsTargets = CreateObjectBasedSearchTarget(dbObjects, dbColumnsByTableId);
        var dbColumnsTargets = dbColumns.Select(p => new ColumnSearchTarget(p));
        var dbIndicesTargets = dbIndices.Select(p => new IndexSearchTarget(p)).ToArray();

        var list = new List<ISearchTarget>();
        list.AddRange(dbObjectsTargets);
        list.AddRange(dbColumnsTargets);
        list.AddRange(dbIndicesTargets);
        return Task.FromResult(list.OrderByDescending(p => p.ModificationDateStr).ToArray());
    }

    private IEnumerable<ISearchTarget> CreateObjectBasedSearchTarget(DbObject[] dbObjects, ILookup<long, DbColumn> dbColumnsByTableId) {
        var list = new List<ISearchTarget>();
        foreach (var dbObject in dbObjects) {
            var simplifiedType = DbObjectType.Parse(dbObject.Type);
            if (simplifiedType.Category == DbSimplifiedType.Constraint) list.Add(new ConstraintSearchTarget(dbObject));
            else if (simplifiedType.Category == DbSimplifiedType.Table) list.Add(new TableSearchTarget(dbObject, dbColumnsByTableId[dbObject.ObjectId].ToArray()));
            else if (simplifiedType.Category == DbSimplifiedType.Other) list.Add(new OtherSearchTarget(dbObject));
            else list.Add(new ObjectSearchTarget(dbObject));
        }
        return list;
    }

    private static void MapIndicesColumns(DbIndex[] dbIndices, DbIndexColumn[] dbIndicesColumns) {
        var columnsByIndexId = dbIndicesColumns.ToLookup(p => ValueTuple.Create(p.OwnerId, p.IndexNumber));
        foreach (var index in dbIndices) {
            index.Columns = columnsByIndexId[ValueTuple.Create(index.OwnerId, index.IndexNumber)].ToArray();
        }
    }

    private void MapDbObjectParents(Dictionary<long, DbObject> dbobjecstByID) {
        foreach (var obj in dbobjecstByID.Values) {
            if (obj.ParentObjectId.HasValue)
                obj.Parent = dbobjecstByID[obj.ParentObjectId.Value];
        }
    }

    private void MapdbColumnsParents(Dictionary<long, DbObject> dbobjecstByID, DbColumn[] dbColumns) {
        foreach (var column in dbColumns) {
            column.Parent = dbobjecstByID[column.TableId];
        }
    }

    private void MapdbIndicesParents(Dictionary<long, DbObject> dbobjecstByID, DbIndex[] dbIndices) {
        foreach (var index in dbIndices) {
            index.Parent = dbobjecstByID[index.OwnerId];
        }
    }
}
