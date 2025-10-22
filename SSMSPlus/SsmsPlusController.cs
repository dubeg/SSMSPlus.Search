using System;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using System.Windows;
using EnvDTE80;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Shell;
using SSMSPlus.Core;
using SSMSPlus.Core.App;
using SSMSPlus.Core.Di;
using SSMSPlus.Core.Integration;
using SSMSPlus.Core.Utils;
using SSMSPlus.Services;
using SSMSPlus.Search;
using MessageBox = System.Windows.MessageBox;

namespace SSMSPlus;

public class SsmsPlusController {
    private static DTE2 _dte;
    private static AsyncPackage _asyncPackage;
    private static OleMenuCommandService _commandService;
    private static IConfigurationRoot _configuration;
    public Microsoft.Extensions.DependencyInjection.ServiceProvider ServiceProvider { get; private set; }
    
    public static SsmsPlusController Instance { get; private set; }

    private SsmsPlusController(AsyncPackage package) {
        ThreadHelper.ThrowIfNotOnUIThread();
        try {
            //_configuration = new ConfigurationBuilder()
            //    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            //    //.AddJsonFile("appsettings.json", optional: false)
            //    .Build();
            var services = new ServiceCollection();
            services.AddLogging(builder => {
                builder.SetMinimumLevel(LogLevel.Trace);
                // Console logging?
            });
            services.AddSingleton<PackageProvider>((_) => new PackageProvider(_dte, _asyncPackage, _commandService));
            services.AddSingleton<IWorkingDirProvider, SSMSWorkingDirProvider>();
            services.AddSingleton<IVersionProvider, VersionProvider>();
            services.AddSSMSPlusCoreServices();
            services.AddSSMSPlusSearchServices();
            ServiceProvider = services.BuildServiceProvider();
            ServiceLocator.SetLocatorProvider(ServiceProvider);
        }
        catch (Exception ex) {
            MessageBox.Show(ex.GetFullStackTraceWithMessage(), "Could not Load SSMSPlus");
            ServiceProvider.GetRequiredService<ILogger<SsmsPlusController>>().LogCritical(ex, "Critical Error when starting pluging");
            throw;
        }
    }

    public static async Task InitializeAsync(AsyncPackage package, DTE2 dte) {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
        _dte = dte;
        _asyncPackage = package;
        _commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        Instance = new SsmsPlusController(package);
    }
}
