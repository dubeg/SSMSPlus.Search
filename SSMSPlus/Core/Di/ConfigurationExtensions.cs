using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SSMSPlus.Core.Utils.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMSPlus.Core.Di
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureSection<TSection>(this IServiceCollection services, IConfiguration config) where TSection : class, IValidatable<TSection>, new()
        {
            services.Configure<TSection>(config);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<TSection>>().Value.Validate());
        }
    }
}
