using E_Commerce.Domain.Contracts;

namespace E_Commerce.API.Extensions
{
    public static class WepApplicationsExtensions
    {
        public static async Task<WebApplication> SeedAndMigrateDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("catalog");
            var identitySeeder = scope.ServiceProvider.GetRequiredKeyedService<IDataSeeder>("identity");
            await seeder.SeedDataAsync();
            await identitySeeder.SeedDataAsync();
            return app;
        }
    }
}
