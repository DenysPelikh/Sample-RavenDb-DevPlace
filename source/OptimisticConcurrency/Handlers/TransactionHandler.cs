using System;
using System.Threading;
using Raven.Abstractions.Exceptions;
using Raven.Client;

namespace OptimisticConcurrency.Handlers
{
    public class TransactionHandler : ITransactionHandler
    {
        private readonly IDocumentStore _store;

        public string Id { get; }

        private Action<string> _writeInfo = s => { };

        public Action<string> WriteInfo
        {
            get
            {
                return _writeInfo;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _writeInfo = value;
            }
        }

        private TimeSpan _transactionDuration = TimeSpan.FromMilliseconds(3000);

        public TimeSpan TransactionDuration
        {
            get
            {
                return _transactionDuration;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _transactionDuration = value;
            }
        }

        public TransactionHandler(string handlerId, IDocumentStore store)
        {
            if (string.IsNullOrWhiteSpace(handlerId)) throw new ArgumentNullException(nameof(handlerId));
            if (store == null) throw new ArgumentNullException(nameof(store));
 
            Id = handlerId;
            _store = store;
        }

        public virtual void Handle(string entityId)
        {
            LogWithIndicators("Start Handle.");

            try
            {
                LogWithIndicators($"Try change entity '{entityId}'.");

                using (var session = _store.OpenSession())
                {
                    var entity = session.Load<TestEntity>(entityId);

                    entity.Name = $"Handle by {Id}";

                    LogWithIndicators("Do some kind of long operation.");
                    Thread.Sleep(_transactionDuration);

                    entity.Number++;

                    LogWithIndicators("Save Changes.");

                    session.SaveChanges();
                }
            }
            catch (ConcurrencyException ex)
            {
                LogWithIndicators(ex.Message);
            }

            LogWithIndicators("Finish Handle.");
        }

        protected virtual void LogWithIndicators(string message)
        {
            WriteInfo(message + $" HandlerId:{Id} CurrentThread:{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
