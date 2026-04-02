using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;
using SSMSPlus.Core.Utils;
using SSMSPlus.Search.Services;

namespace SSMSPlus.Commands;

internal sealed class SearchCommand {
    public static void Initialize(OleMenuCommandService commandService) {
        var cmdId = new CommandID(PackageGuids.SSMSPlus, PackageIds.SearchCommand);
        var menuCommand = new OleMenuCommand(Execute, cmdId);
        commandService.AddCommand(menuCommand);
    }

    private static void Execute(object sender, EventArgs e) {
        try {
            var searchUi = SsmsPlusController.Instance.ServiceProvider.GetRequiredService<SearchUi>();
            searchUi.Execute();
        }
        catch (Exception ex) {
            System.Windows.MessageBox.Show(ex.GetFullStackTraceWithMessage(), "Could not open Search Window");
        }
    }
}
