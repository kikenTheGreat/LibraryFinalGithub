using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Library_Final.Data;
using Library_Final.Interfaces;
using Library_Final.Services;

namespace Library_Final.Utilities
{
    public static class ServiceProvider
    {
        private static IServiceProvider? _serviceProvider;

        public static void Initialize()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Database Context
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=(localdb)\\mssqllocaldb;Database=LibraryManagement;Trusted_Connection=true;MultipleActiveResultSets=true";
            
            services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IBorrowService, BorrowService>();

            // Utilities
            services.AddSingleton<SessionManager>();
        }

        public static T GetService<T>() where T : notnull
        {
            if (_serviceProvider == null)
                throw new InvalidOperationException("ServiceProvider not initialized. Call Initialize() first.");

            return _serviceProvider.GetRequiredService<T>();
        }

        public static void DisposeServices()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
