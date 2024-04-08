using Core.Utilities.IOC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection CreateServices(this IServiceCollection services, ICoreModule[] modules)
        {
            foreach (var module in modules)
                module.Load(services);

            return ServiceTool.Create(services);
        }
    }
}
