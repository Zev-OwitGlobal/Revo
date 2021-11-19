
using Revo.Domain.Entities;
using Revo.Infrastructure.Projections;
using Revo.MongoDB.DataAccess;

namespace Revo.MongoDB.Projections
{
    public class MongoDBEntityEventToDocumentProjector<TSource, TTarget> :
    CrudEntityEventToPocoProjector<TSource, TTarget>,
    IMongoDBDocumentEventProjector<TSource>
    where TSource : class, IAggregateRoot
    where TTarget : class, new()
    {
        protected new IMongoDBCrudRepository Repository { get; }

        public MongoDBEntityEventToDocumentProjector(IMongoDBCrudRepository repository)
        : base(repository)
        {
            Repository = repository;
        }
    }
}
