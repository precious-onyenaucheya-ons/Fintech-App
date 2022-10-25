using Newtonsoft.Json;
using Payment.Core.Interfaces;
using Payment.Domain.Models;

namespace Payment.Infrastructure
{
    public class SeedPaymentInitialData : ISeedPaymentInitialData
    {
        private readonly PaymentDbContext _dbContext;

        public SeedPaymentInitialData(PaymentDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Seed()
        {
            var baseDir = Directory.GetCurrentDirectory();
            await _dbContext.Database.EnsureCreatedAsync();
            if (!_dbContext.Banks.Any())
            {
                string json = File.ReadAllText(Path.Combine(baseDir, "dbSeed/Bank.json"));
                var banks = JsonConvert.DeserializeObject<List<Bank>>(json);
                await _dbContext.AddRangeAsync(banks);
            }
            if (!_dbContext.Wallets.Any())
            {
                string json = File.ReadAllText(Path.Combine(baseDir, "dbSeed/wallet.json"));
                var wallets = JsonConvert.DeserializeObject<List<Wallet>>(json);
                await _dbContext.AddRangeAsync(wallets);                
            }
            await _dbContext.SaveChangesAsync();

        }
    }
}
