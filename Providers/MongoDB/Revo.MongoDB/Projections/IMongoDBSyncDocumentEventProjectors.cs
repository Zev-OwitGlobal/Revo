
using Revo.Domain.Entities;
using Revo.Infrastructure.Projections;

namespace Revo.MongoDB.Projections
{
    public interface IMongoDBSyncDocumentEventProjector<TAggregateRoot> : IEntityEventProjector
    where TAggregateRoot : IAggregateRoot
    {
    }
}
