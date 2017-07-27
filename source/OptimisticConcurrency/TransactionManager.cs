using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OptimisticConcurrency.Handlers;

namespace OptimisticConcurrency
{
    public class TransactionManager
    {
        private readonly Queue<TransactionDate> _transactions;

        public IEnumerable<ITransactionHandler> TransactionHandlers
        {
            get
            {
                return _transactions.Select(date => date.Handler);
            }
        }

        public TransactionManager()
        {
            _transactions = new Queue<TransactionDate>();
        }

        public void AddTransactionToQueue(ITransactionHandler handler, int iterations = 1)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _transactions.Enqueue(new TransactionDate(handler, iterations));
        }

        public void RunTransaction(string workEntityId, TimeSpan delay)
        {
            if (string.IsNullOrWhiteSpace(workEntityId)) throw new ArgumentNullException(nameof(workEntityId));

            while (_transactions.Any())
            {
                var transaction = _transactions.Dequeue();

                Task.Run(() =>
                {
                    for (var i = 0; i < transaction.IterationsOfTransactions; i++)
                    {
                        transaction.Handler.Handle(workEntityId);
                    }
                });

                Thread.Sleep(delay);
            }
        }

        private class TransactionDate
        {
            public ITransactionHandler Handler { get; }
            public int IterationsOfTransactions { get; }

            public TransactionDate(ITransactionHandler handler, int iterationsOfTransactions)
            {
                if (handler == null) throw new ArgumentNullException(nameof(handler));

                Handler = handler;
                IterationsOfTransactions = iterationsOfTransactions;
            }
        }
    }
}
