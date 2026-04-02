global using System;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using SSMSPlus.Commands;
using SSMSPlus.Search.UI;

namespace SSMSPlus;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[ProvideToolWindow(typeof(SearchToolWindow), Window = "00000000-0000-0000-0000-000000000002", MultiInstances = true, Transient = true)]
[Guid(PackageGuids.SSMSPlusString)]
public sealed class SSMSPlusPackage : AsyncPackage {
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dte = await GetServiceAsync(typeof(EnvDTE.DTE)) as EnvDTE80.DTE2;
        var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        SearchCommand.Initialize(commandService);

        await SsmsPlusController.InitializeAsync(this, dte);
    }
}
