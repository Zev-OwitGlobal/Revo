using Revo.DataAccess.Entities;
using Revo.MongoDB.Contexts;

namespace Revo.MongoDB.DataAccess
{
    public class MongoDBCrudRepositoryFactory :
        ICrudRepositoryFactory<IMongoDBCrudRepository>,
        ICrudRepositoryFactory<ICrudRepository>,
        ICrudRepositoryFactory<IReadRepository>
    {
        private readonly IMongoDBContext _context;

        public MongoDBCrudRepositoryFactory(IMongoDBContext context)
        {
            _context = context;
        }

        public IMongoDBCrudRepository Create()
        {
            return new MongoDBCrudRepository(_context);
        }

        ICrudRepository ICrudRepositoryFactory<ICrudRepository>.Create()
        {
            return Create();
        }

        IReadRepository ICrudRepositoryFactory<IReadRepository>.Create()
        {
            return Create();
        }
    }
}
