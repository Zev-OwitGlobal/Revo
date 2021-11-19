using MongoDB.Driver;

namespace Revo.MongoDB.Contexts
{
    /// <summary>
    /// This is the interface of the IMongoDbContext which is managed by the <see cref="BaseMongoRepository"/>.
    /// </summary>
    public interface IMongoDBContext
    {
        /// <summary>
        /// The IMongoClient from the official MongoDb driver
        /// </summary>
        IMongoClient Client { get; }
        /// <summary>
        /// The IMongoDatabase from the official Mongodb driver
        /// </summary>
        IMongoDatabase Database { get; }
        /// <summary>
        /// Returns a collection for a document type that has a partition key.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        IMongoCollection<TDocument> GetCollection<TDocument>()
        where TDocument : class;
        /// <summary>
        /// Returns a collection for a document type using its name.
        /// </summary>
        /// <param name="name">
        /// The name of the collection to get.
        /// </param>
        /// <typeparam name="TDocument">
        /// The Document type the document's collection represents.
        /// </typeparam>
        IMongoCollection<TDocument> GetCollection<TDocument>(string name)
        where TDocument : class;
        /// <summary>
        /// Drops a collection having a partitionkey, use very carefully.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        void DropCollection<TDocument>()
        where TDocument : class;
        /// <summary>
        /// Drops a collection based on the specified name.
        /// </summary>
        void DropCollection(string name);
        /// <summary>
        /// Initializes the connection based on the specified connection string and 
        /// database name.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to use to connect to the database.
        /// </param>
        /// <param name="databaseName">
        /// The database name to connect to.
        /// </param>
        void InitializeConnection(string connectionString, string databaseName);
    }
}
