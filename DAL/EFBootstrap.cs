using Edu_Block.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edu_Block.DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EduBlockDataContext>
    {
        public EduBlockDataContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EduBlockDataContext>();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new EduBlockDataContext(optionsBuilder.Options);
        }
    }

    public static class EFBootstrap
    {
        public static void Configure(IServiceCollection services, IConfiguration config)
        {
            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 443; // Set the HTTPS port for redirection
            });

            services.AddDbContext<EduBlockDataContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Edu_Block_dev")));

            services.AddTransient<DbContext, EduBlockDataContext>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<Util>();
        }
    }
}
