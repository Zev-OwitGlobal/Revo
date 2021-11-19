using Revo.Core.Transactions;

namespace Revo.MongoDB.Repositories
{
    /// <summary>
    /// Represents an interface for a MongoDB implementation of
    /// an <see cref="ITransactionCoordinator"/>.
    /// </summary>
    public interface IMongoDBTransactionCoordinator : ITransactionCoordinator, ITransaction
    {
    }
}
