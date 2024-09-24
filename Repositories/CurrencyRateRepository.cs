using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyConverterAPI.Data;
using CurrencyConverterAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace CurrencyConverterAPI.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyDbContext _context;

        public CurrencyRepository(CurrencyDbContext context)
        {
            _context = context;
        }

        public async Task SaveCurrencyRateAsync(CurrencyRate currencyRate)
        {
            try
            {
                // Add the currency rate to the context
                await _context.CurrencyRates.AddAsync(currencyRate);
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions
                throw new Exception("An error occurred while saving the currency rate.", ex);
            }
        }

        public async Task SaveConversionHistoryAsync(ConversionHistory conversionHistory)
        {
            try
            {
                // Add the conversion history to the context
                await _context.ConversionHistories.AddAsync(conversionHistory);
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions
                throw new Exception("An error occurred while saving the conversion history.", ex);
            }
        }

        public async Task<List<ConversionHistory>> GetConversionHistory()
        {
            try
            {
                // Retrieve and return the conversion history from the database
                return await _context.ConversionHistories.ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions
                throw new Exception("An error occurred while retrieving the conversion history.", ex);
            }
        }
    }
}
