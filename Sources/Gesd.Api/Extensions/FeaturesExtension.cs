using System.Reflection;

using MediatR;

namespace Gesd.Api.Extensions
{
    public static class FeaturesExtension
    {
        public static IServiceCollection AddFeaturesConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
