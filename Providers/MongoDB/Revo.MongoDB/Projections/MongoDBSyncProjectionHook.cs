using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Revo.Core.Commands;
using Revo.Core.Events;
using Revo.Core.Transactions;
using Revo.Domain.Events;

namespace Revo.MongoDB.Projections
{
    public class MongoDBSyncProjectionHook : ITransactionParticipant
    {
        private readonly ICommandContext _commandContext;
        private readonly IMongoDBProjectionSubSystem _projectionSubSystem;
        private readonly List<IEventMessage> _projectedEvents = new List<IEventMessage>();

        public MongoDBSyncProjectionHook(ICommandContext commandContext, IMongoDBProjectionSubSystem projectionSubSystem)
        {
            _commandContext = commandContext;
            _projectionSubSystem = projectionSubSystem;
        }

        public async Task OnBeforeCommitAsync()
        {
            if (_commandContext.UnitOfWork != null)
            {
                var newEvents = _commandContext.UnitOfWork.EventBuffer.Events
                    .SkipWhile((@event, index) => _projectedEvents.Count > index && _projectedEvents[index] == @event)
                    .ToArray();

                if (newEvents.Length > 0)
                {
                    _projectedEvents.AddRange(newEvents);

                    await _projectionSubSystem.ExecuteProjectionsAsync(
                     _commandContext
                         .UnitOfWork
                         .EventBuffer
                         .Events
                         .OfType<IEventMessage<DomainAggregateEvent>>()
                         .ToArray(),
                     _commandContext.UnitOfWork,
                     new MongoDBEventProjectionOptions(true));
                }
            }
        }

        public Task OnCommitSucceededAsync()
        {
            _projectedEvents.Clear();
            return Task.CompletedTask;
        }

        public Task OnCommitFailedAsync()
        {
            _projectedEvents.Clear();
            return Task.CompletedTask;
        }
    }
}
