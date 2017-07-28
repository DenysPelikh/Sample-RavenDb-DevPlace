using System;
using OptimisticConcurrency.Handlers;

namespace OptimisticConcurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = Setup.GetLogging();

            using (var store = Setup.SetupEmbeddableStore())
            {
                var transactionManager = new TransactionManager(logger);
                var entity = new TestEntity();

                using (var session = store.OpenSession())
                {
                    session.Store(entity);
                    session.SaveChanges();
                }

                transactionManager.AddTransactionToQueue(new TransactionHandler("FirstHandler", store) { WriteInfo = s => logger.Information(s) });
                transactionManager.AddTransactionToQueue(new TransactionHandler("SecondHandler", store) { WriteInfo = s => logger.Information(s) });

                transactionManager.RunTransaction(entity.Id, TimeSpan.FromMilliseconds(1000));
            }

            Console.ReadLine();
        }
    }
}
