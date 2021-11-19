using System.Collections.Generic;
using Revo.Core.Commands;
using Revo.Core.Events;
using Revo.Core.Transactions;
using Revo.Domain.Events;
using Revo.Infrastructure.Events.Async;
using Revo.Infrastructure.Projections;

namespace Revo.MongoDB.Projections
{
    public class MongoDBProjectionEventListener : ProjectionEventListener
    {
        public override IAsyncEventSequencer EventSequencer { get; }
        public MongoDBProjectionEventListener(
            IMongoDBProjectionSubSystem projectionSubSystem,
            IUnitOfWorkFactory unitOfWorkFactory,
            CommandContextStack commandContextStack,
            MongoDBProjectionEventSequencer eventSequencer
            ) :
            base(projectionSubSystem, unitOfWorkFactory, commandContextStack)
        {
            EventSequencer = eventSequencer;
        }

        public class MongoDBProjectionEventSequencer : AsyncEventSequencer<DomainAggregateEvent>
        {
            public readonly string QueueNamePrefix = "MongoDBProjectionEventSequencer:";
            protected override IEnumerable<EventSequencing> GetEventSequencing(IEventMessage<DomainAggregateEvent> message)
            {
                yield return new EventSequencing()
                {
                    SequenceName = $"{QueueNamePrefix}{message.Event.AggregateId}",
                    EventSequenceNumber = message.Metadata.GetStreamSequenceNumber(),
                };
            }
            protected override bool ShouldAttemptSynchronousDispatch(IEventMessage<DomainAggregateEvent> message)
            {
                return true;
            }
        }

    }
}
