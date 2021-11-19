using System.Threading.Tasks;

using Revo.Core.Events;
using Revo.Core.Transactions;
using Revo.DataAccess.Entities;
using Revo.Domain.Entities;
using Revo.Infrastructure.Events;
using Revo.Infrastructure.EventStores;
using Revo.Infrastructure.Repositories;

namespace Revo.MongoDB.Repositories
{
    public class MongoDBEventSourcedAggregateStore : EventSourcedAggregateStore, ITransactionParticipant
    {
        private readonly IMongoDBTransactionCoordinator _transactionCoordinator;

        public override bool NeedsSave => false;
        public MongoDBEventSourcedAggregateStore(
            IEventStore eventStore,
            IEntityTypeManager entityTypeManager,
            IPublishEventBuffer publishEventBuffer,
            IEventMessageFactory eventMessageFactory,
            IEventSourcedAggregateFactory eventSourcedAggregateFactory,
            IMongoDBTransactionCoordinator transactionCoordinator,
            IRepositoryFilter[] repositoryFilters
        ) : base(eventStore, entityTypeManager, publishEventBuffer, repositoryFilters, eventMessageFactory, eventSourcedAggregateFactory)
        {
            _transactionCoordinator = transactionCoordinator;
            _transactionCoordinator.AddTransactionParticipant(this);
        }

        public override Task SaveChangesAsync()
        {
            return _transactionCoordinator.CommitAsync();
        }
        public Task OnBeforeCommitAsync()
        {
            return base.SaveChangesAsync();
        }

        public Task OnCommitSucceededAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnCommitFailedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
