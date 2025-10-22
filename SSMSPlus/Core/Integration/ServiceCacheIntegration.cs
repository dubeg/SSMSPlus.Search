using EnvDTE;
using Microsoft.SqlServer.Management.UI.VSIntegration;

namespace SSMSPlus.Core.Integration {

    public class ServiceCacheIntegration : IServiceCacheIntegration
    {
        public void OpenScriptInNewWindow(string script)
        {
            ServiceCache.ScriptFactory.CreateNewBlankScript(Microsoft.SqlServer.Management.UI.VSIntegration.Editors.ScriptType.Sql);

            TextDocument doc = (EnvDTE.TextDocument)ServiceCache.ExtensibilityModel.Application.ActiveDocument.Object(null);
            doc.EndPoint.CreateEditPoint().Insert(script);
        }
    }
}
