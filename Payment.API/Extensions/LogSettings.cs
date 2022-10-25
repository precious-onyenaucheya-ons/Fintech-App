using Raven.Client.Documents;
using Serilog;
using Serilog.Exceptions;
using Serilog.Events;
using System.Security.Cryptography.X509Certificates;

namespace Payment.API.Extensions
{
    public class LogSettings
    {
        public static void SetUpSerilog(IConfiguration config)
        {
            DocumentStore ravenStore = new()
            {
                Urls = new string[] { config["RavenDbConfigurations:ConnectionURL"] },
                Database = config["RavenDbConfigurations:DatabaseName"]
            };

            ravenStore.Certificate = new X509Certificate2(config["RavenDbConfigurations:CertificatePath"],
            config["RavenDbConfigurations:Password"], X509KeyStorageFlags.MachineKeySet);

            ravenStore.Initialize();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: @"./Logs/log-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information
                )
                .Enrich.WithExceptionDetails()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.FromLogContext()
                .WriteTo.RavenDB(ravenStore)
                .CreateLogger();
        }
    }
}
