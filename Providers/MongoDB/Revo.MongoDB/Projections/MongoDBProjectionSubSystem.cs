using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Revo.Core.Events;
using Revo.Core.Transactions;
using Revo.Domain.Entities;
using Revo.Domain.Events;
using Revo.Infrastructure.Events;
using Revo.Infrastructure.Projections;
using Revo.MongoDB.Repositories;

namespace Revo.MongoDB.Projections
{
    public class MongoDBProjectionSubSystem : ProjectionSubSystem, IMongoDBProjectionSubSystem, ITransactionParticipant
    {
        private readonly IMongoDBProjectorResolver _projectorResolvers;
        private readonly Lazy<IMongoDBTransactionCoordinator> _transactionCoordinator;
        private readonly HashSet<IEntityEventProjector> _allUsedProjectors = new HashSet<IEntityEventProjector>();
        private bool _transactionParticipantRegistered = false;
        private EventProjectionOptions _eventProjectionOptions;

        public MongoDBProjectionSubSystem(
            IEntityTypeManager entityTypeManager,
            IEventMessageFactory eventMessageFactory,
            IMongoDBProjectorResolver projectorResolvers,
            Lazy<IMongoDBTransactionCoordinator> transactionCoordinator
        ) : base(entityTypeManager, eventMessageFactory)
        {
            _projectorResolvers = projectorResolvers;
            _transactionCoordinator = transactionCoordinator;
        }

        public override async Task ExecuteProjectionsAsync(
            IReadOnlyCollection<IEventMessage<DomainAggregateEvent>> events,
            IUnitOfWork unitOfWork, EventProjectionOptions options)
        {
            if (!_transactionParticipantRegistered)
            {
                _transactionParticipantRegistered = true;
                _transactionCoordinator.Value.AddTransactionParticipant(this);
            }

            await base.ExecuteProjectionsAsync(events, unitOfWork, options);
        }

        protected override IEnumerable<IEntityEventProjector> GetProjectors(Type entityType, EventProjectionOptions options)
        {
            var mongoDBOptions = options as MongoDBEventProjectionOptions;

            if (mongoDBOptions == null || mongoDBOptions.IsSynchronousProjection == false)
            {
                return _projectorResolvers.GetProjectors(entityType);
            }

            return _projectorResolvers.GetSyncProjectors(entityType);
        }
        protected override async Task CommitUsedProjectorsAsync(IReadOnlyCollection<IEntityEventProjector> usedProjectors, EventProjectionOptions options)
        {
            _eventProjectionOptions = options;

            foreach (var projector in usedProjectors)
            {
                _allUsedProjectors.Add(projector);
            }

            await _transactionCoordinator.Value.CommitAsync();
        }

        public async Task OnBeforeCommitAsync()
        {
            await base.CommitUsedProjectorsAsync(_allUsedProjectors, _eventProjectionOptions);
        }

        public Task OnCommitSucceededAsync()
        {
            _allUsedProjectors.Clear();
            _eventProjectionOptions = null;

            return Task.CompletedTask;
        }

        public Task OnCommitFailedAsync()
        {
            _allUsedProjectors.Clear();
            _eventProjectionOptions = null;

            return Task.CompletedTask;
        }
    }
}
