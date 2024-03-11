using Gesd.Api.Services;
using Gesd.Api.Services.Contrats;

namespace Gesd.Api.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServiceConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFichierService, FichierService>();
            return services;
        }
    }
}
