using Gesd.Api.Context;
using Gesd.Api.Repositories.Contrats;
using Gesd.Api.Repositories;
using Gesd.Api.Settings;
using Microsoft.EntityFrameworkCore;

namespace Gesd.Api.Extensions
{
    public static class DatasExtensions
    {
        public static IServiceCollection AddDataConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            //var sqlSettings = configuration.GetSection(nameof(SQLSettings)).Get<SQLSettings>();
            //var connectionstring = $"Server={sqlSettings.Server},{sqlSettings.Port};Initial Catalog={sqlSettings.Database};Persist Security Info=False;User ID={sqlSettings.UserName};Password={sqlSettings.Password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var connectionstring = "Server=tcp:sql-server-vdl.database.windows.net,1433;Initial Catalog=az-database-vdl-;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            services.AddDbContext<GesdContext>(options =>
            {
                options.UseSqlServer(connectionstring);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var fichierSettings = configuration.GetSection(nameof(FileSettings)).Get<FileSettings>();
            services.AddSingleton(fichierSettings);

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IEncryptedFileRepository, EncryptedFileRepository>();
            services.AddScoped<IBlobRepository, BlobRepository>();
            services.AddScoped<IKeyStoreRepository, KeyStoreRepository>();

            // Ajouter la migration de la base de données ici
            //using (var serviceProvider = services.BuildServiceProvider())
            //{
            //    var dbContext = serviceProvider.GetRequiredService<GesdContext>();
            //    dbContext.Database.Migrate();
            //}

            return services;
        }
    }
}
