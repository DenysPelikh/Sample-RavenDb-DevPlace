using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace OptimisticConcurrency
{
    public class Setup
    {
        public static IDocumentStore SetupEmbeddableStore()
        {
            var store = new EmbeddableDocumentStore
            {
                DataDirectory = @"Data",
                RunInMemory = true,
                DefaultDatabase = "TestDb",
                Conventions =
                {
                    DefaultUseOptimisticConcurrency = true
                }
            };

            store.Configuration.Storage.Voron.AllowOn32Bits = true;

            store.Initialize();

            return store;
        }

        public static IDocumentStore SetupStore()
        {
            var store = new DocumentStore
            {
                DefaultDatabase = "TestDb",
                ConnectionStringName = "Test",
                Conventions =
                {
                    DefaultUseOptimisticConcurrency = true
                }
            };

            store.Initialize();

            return store;
        }

        public static ILogger GetLogging()
        {
            var outputTemplate = "{Timestamp:HH:mm:ss} [{Level}] [ProcessId:{ProcessId}] [ThreadId:{ThreadId}] {Message}{NewLine}{Exception}";
            var levelSwitch = new LoggingLevelSwitch { MinimumLevel = LogEventLevel.Debug };

            var configuration = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .WriteTo.Console(theme: SystemConsoleTheme.Literate, outputTemplate: outputTemplate)
                .WriteTo.Trace(outputTemplate: outputTemplate);

            return configuration.CreateLogger();
        }
    }
}
