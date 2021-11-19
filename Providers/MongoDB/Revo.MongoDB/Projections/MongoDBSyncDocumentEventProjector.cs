using Revo.Domain.Entities;
using Revo.MongoDB.DataAccess;

namespace Revo.MongoDB.Projections
{
    /// <summary>
    /// A synchronous MongoDB CRUD repository-backed event projector for an aggregate type with arbitrary read-model(s).
    /// A convention-based abstract base class that calls an Apply for every event type
    /// and also supports sub-projectors.
    /// </summary>
    public class MongoDBSyncDocumentEventProjector<TSource> : MongoDBDocumentEventProjector, IMongoDBSyncDocumentEventProjector<TSource>
    where TSource : class, IAggregateRoot
    {
        public MongoDBSyncDocumentEventProjector(IMongoDBCrudRepository repository) : base(repository)
        {
        }
    }
}
