using Raven.Client.Documents;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using System.Security.Cryptography.X509Certificates;

namespace NotificationServices.API
{
    public static class LogSettings
    {
        public static void SetUpSerilog(IConfiguration config)
        {
            DocumentStore ravenStore = new DocumentStore
            {
                Urls = new string[] { config["RavenDbConfigurations:ConnectionURL"] },
                Database = config["RavenDbConfigurations:DatabaseName"]
            };

            ravenStore.Certificate = new X509Certificate2(config["RavenDbConfigurations:CertificatePath"],
            config["RavenDbConfigurations:Password"], X509KeyStorageFlags.MachineKeySet);

            ravenStore.Initialize();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: @"Logs/logs-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information
                )
                .Enrich.WithExceptionDetails()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .WriteTo.RavenDB(ravenStore)
                .CreateLogger();
        }
    }
}