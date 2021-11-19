
using System;
using Revo.Core.Events;
using Revo.Core.Transactions;
using Revo.Domain.Entities;
using Revo.Infrastructure.Events;
using Revo.Infrastructure.EventStores;
using Revo.Infrastructure.Repositories;

namespace Revo.MongoDB.Repositories
{
    /// <summary>
    /// Represents a factory class for creating instances of 
    /// an <see cref="IAggregateStore"/> for MongoDB.
    /// </summary>
    public class MongoDBEventSourcedAggregateStoreFactory : EventSourcedAggregateStoreFactory
    {
        private readonly Func<IPublishEventBuffer, IMongoDBTransactionCoordinator, MongoDBEventSourcedAggregateStore> _func;
        private readonly IMongoDBTransactionCoordinator _transactionCoordinator;


        /// <summary>
        /// Creates an instance of the <see cref="MongoDBEventSourcedAggregateStoreFactory"/>.
        /// </summary>
        /// <param name="eventStore"></param>
        /// <param name="entityTypeManager"></param>
        /// <param name="publishEventBuffer"></param>
        /// <param name="eventMessageFactory"></param>
        /// <param name="eventSourcedAggregateFactory"></param>
        /// <param name="transactionCoordinator"></param>
        /// <returns></returns>
        public MongoDBEventSourcedAggregateStoreFactory(
            Func<IPublishEventBuffer, IMongoDBTransactionCoordinator, MongoDBEventSourcedAggregateStore> func,
            IMongoDBTransactionCoordinator transactionCoordinator)
        : base(null)
        {
            _func = func;
        }

        /// <summary>
        /// Creates an <see cref="IAggregateStore"/> using the specified <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <param name="unitOfWork">
        /// The <see cref="IUnitOfWork"/> to use within the <see cref="IAggregateStore"/>.
        /// </param>
        /// <returns>
        /// Returns an instance of an <see cref="IAggregateStore"/> using the specified <see cref="IUnitOfWork"/>.
        /// </returns>
        public override IAggregateStore CreateAggregateStore(IUnitOfWork unitOfWork)
        {
            return _func(unitOfWork?.EventBuffer, _transactionCoordinator);
        }
    }
}
