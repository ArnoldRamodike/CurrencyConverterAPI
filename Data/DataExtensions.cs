using Microsoft.EntityFrameworkCore;

namespace CurrencyConverterAPI.Data
{
    public static class DataExtensions
    {
        public static async Task MigrateDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}