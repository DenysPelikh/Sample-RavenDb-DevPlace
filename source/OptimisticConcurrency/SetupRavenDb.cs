using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace OptimisticConcurrency
{
    public static class SetupRavenDb
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
    }
}
