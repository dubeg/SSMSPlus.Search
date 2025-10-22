global using System;
global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;
using SSMSPlus.Search.UI;

namespace SSMSPlus;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[ProvideToolWindow(typeof(SearchToolWindow), Window = "00000000-0000-0000-0000-000000000002", MultiInstances = true, Transient = true)]
[Guid(PackageGuids.SSMSPlusString)]
public sealed class SSMSPlusPackage : ToolkitPackage {
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
        await this.RegisterCommandsAsync();
        await SsmsPlusController.InitializeAsync(this, await this.GetServiceAsync<EnvDTE.DTE, EnvDTE80.DTE2>());
    }
}