using System;

namespace OptimisticConcurrency.Handlers
{
    public interface ITransactionHandler
    {
        /// <summary>
        /// Handler Id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Duration of transaction
        /// </summary>
        TimeSpan TransactionDuration { get; set; }

        /// <summary> 
        /// Do some work with Entity
        /// </summary>
        /// <param name="entityId">Entity Id with which the transaction</param>
        void Handle(string entityId);
    }
}
