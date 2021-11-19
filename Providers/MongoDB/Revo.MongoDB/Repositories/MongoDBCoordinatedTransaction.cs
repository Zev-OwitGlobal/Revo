using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Revo.Core.Commands;
using Revo.Core.Transactions;
using Revo.MongoDB.DataAccess;
using Revo.MongoDB.EventStores;
using Revo.MongoDB.Projections;

namespace Revo.MongoDB.Repositories
{
    /// <summary>
    /// Represents a class that implements a MongoDB <see cref="CoordinatedTransaction"/>.
    /// </summary>
    public class MongoDBCoordinatedTransaction : CoordinatedTransaction, IMongoDBTransactionCoordinator, IUnitOfWorkListener
    {
        private readonly Lazy<MongoDBSyncProjectionHook> _mongoDBSyncProjectionHook;

        public MongoDBCoordinatedTransaction(
            IMongoDBCrudRepository repository,
            Lazy<MongoDBSyncProjectionHook> mongoDBSyncProjectionHook,
            ICommandContext commandContext
        )
        : base(new MongoDBDocumentStoreTransaction(repository))
        {
            _mongoDBSyncProjectionHook = mongoDBSyncProjectionHook;

            if (commandContext.UnitOfWork?.IsWorkBegun == true)
            {
                commandContext.UnitOfWork.AddInnerTransaction(this);
            }
        }

        /// <summary>
        /// Commits any uncommitted transactions.
        /// </summary>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        protected override async Task DoCommitAsync()
        {
            if (!Participants.Any((participant) => participant is MongoDBSyncProjectionHook))
            {
                Participants.Add(_mongoDBSyncProjectionHook.Value);
            }
            OrderParticipants();
            await base.DoCommitAsync();
        }

        /// <summary>
        /// Raised before the current context's work is committed.
        /// </summary>
        /// <param name="unitOfWork">
        /// Instance of the current <see cref="IUnitOfWork"/> about
        /// to be committed.
        /// </param>
        public Task OnBeforeWorkCommitAsync(IUnitOfWork unitOfWork)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Raised when the work begins against the specified <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <param name="unitOfWork">
        /// Instance of the current <see cref="IUnitOfWork"/> to begin work on.
        /// </param>
        public void OnWorkBegin(IUnitOfWork unitOfWork)
        {
            unitOfWork.AddInnerTransaction(this);
        }

        /// <summary>
        /// Raised when the specified <see cref="IUnitOfWork"/>
        /// was successfully completed.
        /// </summary>
        /// <param name="unitOfWork">
        /// Instance of the current <see cref="IUnitOfWork"/> that succeeded.
        /// </param>
        /// <returns>
        /// </returns>
        public Task OnWorkSucceededAsync(IUnitOfWork unitOfWork)
        {
            return Task.CompletedTask;
        }

        private void OrderParticipants()
        {
            Participants = Participants.OrderBy(x =>
            {
                switch (x)
                {
                    case MongoDBEventSourcedAggregateStore _:
                        return 100;
                    case MongoDBSyncProjectionHook _:
                        return 101;
                    case MongoDBProjectionSubSystem _:
                        return 102;
                    case MongoDBEventStore _:
                        return 103;
                }
                return 0;
            }).ToList();
        }

        /// <summary>
        /// Represents an inner class to keep track of a MongoDB <see cref="ITransaction"/>.
        /// </summary>
        public class MongoDBDocumentStoreTransaction : ITransaction
        {
            private bool _disposed = false;
            private readonly IMongoDBCrudRepository _repository;

            public MongoDBDocumentStoreTransaction(IMongoDBCrudRepository repository)
            {
                _repository = repository;
            }

            public Task CommitAsync()
            {
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // TODO: Dispose transactions.
                }

                _disposed = true;
            }
        }
    }
}
