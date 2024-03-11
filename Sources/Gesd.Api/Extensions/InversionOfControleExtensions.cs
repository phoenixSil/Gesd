using Gesd.Api.Context;

namespace Gesd.Api.Extensions
{
    public static class InversionOfControleExtensions
    {
        public static IServiceCollection AddIocConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(logging =>
            {
                logging.AddConsole(); // Ajoute la sortie console pour la journalisation
            });

            services.AddApplicationServiceConfiguration(configuration);
            services.AddDataConfiguration(configuration);
            services.AddFeaturesConfiguration(configuration);

            return services;
        }
    }
}
