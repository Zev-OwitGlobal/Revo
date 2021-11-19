using MongoDB.Bson;
using MongoDB.Driver;

using Revo.Infrastructure.EventStores.Generic.Model;

namespace Revo.MongoDB.EventStores
{
    public static class MongoDBExtensions
    {
        public static IMongoCollection<EventStreamRow> CreateOrGetCollection(this IMongoDatabase database, string collectionName)
        {
            BsonDocument filter = new BsonDocument("name", collectionName);
            IAsyncCursor<BsonDocument> collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });

            bool doesCollectionExist = collections.Any();

            if (!doesCollectionExist)
            {
                database.CreateCollection(collectionName);
            }

            return database.GetCollection<EventStreamRow>(collectionName);
        }
    }
}
