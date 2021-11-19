using Revo.Domain.Entities;
using Revo.Infrastructure.Projections;
using Revo.MongoDB.DataAccess;

namespace Revo.MongoDB.Projections
{
    /// <summary>
    /// A MongoDB document store-backed event projector with arbitrary read-model(s).
    /// A convention-based abstract base class that calls an Apply for every event type
    /// and also supports sub-projectors.
    /// </summary>
    public abstract class MongoDBDocumentEventProjector : EntityEventProjector
    {
        protected IMongoDBCrudRepository Repository;

        public MongoDBDocumentEventProjector(IMongoDBCrudRepository repository)
        {
            Repository = repository;
        }
    }

    /// <summary>
    /// A MongoDB document store-backed event projector for an aggregate type with arbitrary read-model(s).
    /// A convention-based abstract base class that calls an Apply for every event type
    /// and also supports sub-projectors.
    /// </summary>
    public abstract class MongoDBEntityEventProjector<TSourceAggregateRoot> : MongoDBDocumentEventProjector, IMongoDBDocumentEventProjector<TSourceAggregateRoot>
    where TSourceAggregateRoot : class, IAggregateRoot
    {
        public MongoDBEntityEventProjector(IMongoDBCrudRepository repository)
        : base(repository)
        {
        }
    }
}
