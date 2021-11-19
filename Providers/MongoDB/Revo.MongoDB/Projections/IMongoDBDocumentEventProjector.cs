
using Revo.Domain.Entities;
using Revo.Infrastructure.Projections;

namespace Revo.MongoDB.Projections
{
    public interface IMongoDBDocumentEventProjector<TAggregateRoot> : IEntityEventProjector
    where TAggregateRoot : IAggregateRoot
    {
    }
}
