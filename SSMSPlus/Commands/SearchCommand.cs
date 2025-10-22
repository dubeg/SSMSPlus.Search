using Microsoft.Extensions.DependencyInjection;
using SSMSPlus.Core.Utils;
using SSMSPlus.Search.Services;

namespace SSMSPlus;

[Command(PackageIds.SearchCommand)]
internal sealed class SearchCommand : BaseCommand<SearchCommand> {
    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e) {
        try {
            var searchUi = SsmsPlusController.Instance.ServiceProvider.GetRequiredService<SearchUi>();
            searchUi.Execute();
        }
        catch (Exception ex) { 
            System.Windows.MessageBox.Show(ex.GetFullStackTraceWithMessage(), "Could not open Search Window");
        }
    }
}
