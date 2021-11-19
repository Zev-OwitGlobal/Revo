using System.Threading.Tasks;

using Revo.Core.Transactions;
using Revo.Infrastructure.Events;
using Revo.Infrastructure.EventStores.Generic;
using Revo.MongoDB.Repositories;
using Revo.MongoDB.DataAccess;

namespace Revo.MongoDB.EventStores
{
    public class MongoDBEventStore : EventStore, ITransactionParticipant, IMongoDBEventStore
    {
        private readonly IMongoDBTransactionCoordinator _transactionCoordinator;
        private readonly MongoDBEventStoreSettings _settings;
        public MongoDBEventStore(
            IMongoDBCrudRepository repository,
            IEventSerializer eventSerializer,
            IMongoDBTransactionCoordinator transactionCoordinator)
           : base(repository, eventSerializer)
        {
            _transactionCoordinator = transactionCoordinator;

            transactionCoordinator.AddTransactionParticipant(this);
        }
        public override async Task CommitChangesAsync()
        {
            await _transactionCoordinator.CommitAsync();
        }
        public Task OnBeforeCommitAsync()
        {
            return DoBeforeCommitAsync();
        }

        public Task OnCommitSucceededAsync()
        {
            return DoOnCommitSucceedAsync();
        }

        public Task OnCommitFailedAsync()
        {
            return DoOnCommitFailedAsync();
        }
    }
}
