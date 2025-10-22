using Microsoft.Extensions.DependencyInjection;
using SSMSPlus.Search.Repositories;
using SSMSPlus.Search.Services;
using SSMSPlus.Search.UI;

namespace SSMSPlus.Search; 

public static class ServiceCollectionExtension {
    public static IServiceCollection AddSSMSPlusSearchServices(this IServiceCollection services) {
        services.AddSingleton<SearchUi>();
        services.AddSingleton<IDbIndexer, DbIndexer>();
        services.AddSingleton<SchemaSearchRepository>();
        services.AddTransient<SchemaSearchControlVM>();
        return services;
    }
}
