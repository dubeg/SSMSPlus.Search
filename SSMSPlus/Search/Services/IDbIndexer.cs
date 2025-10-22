using System.Threading.Tasks;
using SSMSPlus.Core.Integration.Connection;
using SSMSPlus.Search.Entities;

namespace SSMSPlus.Search.Services
{
    public interface IDbIndexer
    {
        Task<int> DbExistsAsync(DbConnectionString dbConnectionString);
        Task<int> IndexAsync(DbConnectionString dbConnectionString);
        Task<int> ReIndexAsync(DbConnectionString dbConnectionString);
    }
}