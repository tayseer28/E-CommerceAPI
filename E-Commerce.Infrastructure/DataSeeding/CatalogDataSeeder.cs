using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger) : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if(pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                // path 
                // D:\what to keep\route\Back End\9. API\session 01\E-CommerceAPI\E-Commerce.API\bin\Debug\net8.0\DataSeed\products.json
                var seedRootPath = Path.Combine(AppContext.BaseDirectory, "DataSeed"); // BaseDirectory get the default path (until \net8.0)
                await SeedIfEmptyAsync<ProductBrand, int>(seedRootPath, "brands.json", ct);
                await SeedIfEmptyAsync<ProductType, int>(seedRootPath, "types.json", ct);
                await SeedIfEmptyAsync<Product, int>(seedRootPath, "products.json", ct);

                int result  = await dbContext.SaveChangesAsync(ct);
                if (result > 0)
                    logger.LogInformation($"{result} Rows Added");
                else
                    logger.LogInformation($"Database Already Seeded");
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        private async Task SeedIfEmptyAsync<TEntity, TKey>(string rootPath, string fileName, CancellationToken ct) where TEntity : BaseEntity<TKey> , new()
        {
            if(await dbContext.Set<TEntity>().AnyAsync())
            {
                logger.LogInformation("Table Already Has Data");
                return;
            }
            //check if the file name exist in the rootPath
            var filePath = Path.Combine(rootPath, fileName);
            if(!File.Exists(filePath))
            {
                logger.LogWarning($"File {fileName} is not Exist");
                return;
            }
            using var fileStream = File.OpenRead(filePath);
            var items = await JsonSerializer.DeserializeAsync<List<TEntity>>(fileStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct);

            if (items?.Any() ?? false)
                dbContext.Set<TEntity>().AddRange(items);
        }
    }
}
