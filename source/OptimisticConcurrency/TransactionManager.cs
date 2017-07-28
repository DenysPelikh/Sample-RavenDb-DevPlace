using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OptimisticConcurrency.Handlers;
using Serilog;

namespace OptimisticConcurrency
{
    public class TransactionManager
    {
        private readonly ILogger _logger;

        private readonly Queue<TransactionDate> _transactions;

        public IEnumerable<ITransactionHandler> TransactionHandlers
        {
            get
            {
                return _transactions.Select(date => date.Handler);
            }
        }

        public TransactionManager(ILogger logger)
        {
            _logger = logger;

            _transactions = new Queue<TransactionDate>();
        }

        public void AddTransactionToQueue(ITransactionHandler handler, int iterations = 1)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            _transactions.Enqueue(new TransactionDate(handler, iterations));

            _logger.Information("Transaction Handler {handlerId} with duration {duration} was added to the queue", handler.Id, handler.TransactionDuration);
            _logger.Information("Current items in queue {count}", _transactions.Count);
        }

        public void RunTransaction(string workEntityId, TimeSpan delay)
        {
            if (string.IsNullOrWhiteSpace(workEntityId)) throw new ArgumentNullException(nameof(workEntityId));

            _logger.Information("Run transactions for Entity {workEntityId}", workEntityId);

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

                _logger.Information("Transaction Handler {handlerId} was started with {iterations} iterations", transaction.Handler.Id, transaction.IterationsOfTransactions);

                if (_transactions.Any()) Thread.Sleep(delay);
            }
        }

        private class TransactionDate
        {
            public ITransactionHandler Handler { get; }
            public int IterationsOfTransactions { get; }

            public TransactionDate(ITransactionHandler handler, int iterationsOfTransactions)
            {
                if (handler == null) throw new ArgumentNullException(nameof(handler));
                if (iterationsOfTransactions < 0) throw new ArgumentOutOfRangeException(nameof(iterationsOfTransactions));

                Handler = handler;
                IterationsOfTransactions = iterationsOfTransactions;
            }
        }
    }
}
