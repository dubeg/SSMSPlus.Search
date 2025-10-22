using Microsoft.Extensions.DependencyInjection;
using SSMSPlus.Core.Integration;
using SSMSPlus.Core.Integration.Connection;
using SSMSPlus.Core.Integration.ObjectExplorer;

namespace SSMSPlus.Core {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddSSMSPlusCoreServices(this IServiceCollection services) {
            services.AddSingleton<IObjectExplorerInteraction, ObjectExplorerInteraction>();
            services.AddSingleton<IServiceCacheIntegration, ServiceCacheIntegration>();
            services.AddSingleton<DbConnectionProvider>();
            return services;
        }
    }
}
