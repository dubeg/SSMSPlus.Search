using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using SSMSPlus.Core.Integration;
using SSMSPlus.Core.Integration.Connection;
using SSMSPlus.Search.UI;

namespace SSMSPlus.Search.Services; 

public class SearchUi {
    private int id;
    private PackageProvider _packageProvider;
    DbConnectionProvider _dbConnectionProvider;

    public SearchUi(PackageProvider packageProvider, DbConnectionProvider dbConnectionProvider) {
        _packageProvider = packageProvider;
        _dbConnectionProvider = dbConnectionProvider;
    }
    
    public void Execute() {
        var dbConnectionString = _dbConnectionProvider.GetFromSelectedDatabase();
        if (dbConnectionString == null) {
            dbConnectionString = _dbConnectionProvider.GetFromActiveConnection();
            if (dbConnectionString == null) {
                System.Windows.MessageBox.Show("Please select a user database in object explorer or connect to a user database", "SSMS plus");
                return;
            }
        }
        var toolWindow = _packageProvider.AsyncPackage.FindToolWindow(typeof(SearchToolWindow), id++, true) as SearchToolWindow;
        toolWindow.Intialize(dbConnectionString);
        var frame = (IVsWindowFrame)toolWindow.Frame;
        frame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSFRAMEMODE.VSFM_MdiChild);
        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.Show());
    }
}
