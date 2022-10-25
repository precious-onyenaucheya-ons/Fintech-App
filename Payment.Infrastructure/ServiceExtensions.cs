using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Core.Interfaces;
using Payment.Infrastructure.ExternalServices;
using Payment.Infrastructure.Repositories;

namespace Payment.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void AddPaymentInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PaymentDbContext>(options => 
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), 
                    b => b.MigrationsAssembly(typeof(PaymentDbContext).Assembly.FullName)));

            // Register services here
            services.AddScoped<ISeedPaymentInitialData, SeedPaymentInitialData>(); // Added during seeding
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBankRepository, BankRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IVirtualAccountRepository, VirtualAccountRepository>();
            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddHttpClient<IHttpClientService, HttpClientService>();
            services.AddScoped<IHttpClientService, HttpClientService>();
        }
    }
}
