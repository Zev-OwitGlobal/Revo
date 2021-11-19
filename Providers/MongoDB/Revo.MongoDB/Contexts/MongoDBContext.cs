using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

using Revo.Core.Core;
using Revo.MongoDB.Attributes;

namespace Revo.MongoDB.Contexts
{
    /// <summary>
    /// The MongoDB context
    /// </summary>
    public class MongoDBContext : IMongoDBContext
    {
        private const string DEFAULT_CONVENTION = "camelCase";

        /// <summary>
        /// The IMongoClient from the official MongoDB driver
        /// </summary>
        public IMongoClient Client { get; private set; }

        /// <summary>
        /// The IMongoDatabase from the official MongoDB driver
        /// </summary>
        public IMongoDatabase Database { get; private set; }

        /// <summary>
        /// The constructor of the MongoDBContext, it needs an object implementing <see cref="IMongoDatabase"/>.
        /// </summary>
        /// <param name="mongoDatabase">An object implementing IMongoDatabase</param>
        public MongoDBContext(IMongoDatabase mongoDatabase)
        {
            InitializeGuidRepresentation();
            InitializeConvention();
            Database = mongoDatabase;
            Client = Database.Client;
        }

        /// <summary>
        /// The constructor of the MongoDBContext, it needs a connection string and a database name. 
        /// </summary>
        /// <param name="connectionString">The connections string.</param>
        /// <param name="databaseName">The name of your database.</param>
        public MongoDBContext(string connectionString, string databaseName)
        {
            InitializeGuidRepresentation();
            InitializeConvention();
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(databaseName);
        }

        public MongoDBContext()
        {
            InitializeGuidRepresentation();
        }

        /// <summary>
        /// Initializes an instance of a <see cref="IMongoDBContext"/> using a connection string.
        /// </summary>
        /// <param name="connectionString">
        /// The MongoDB connection string to use for this MongoDBContext.
        /// </param>
        public MongoDBContext(string connectionString)
        : this(connectionString, new MongoUrl(connectionString).DatabaseName)
        {

        }

        /// <summary>
        /// The constructor of the <see cref="MongoDBContext" />, it needs a connection string and a database name. 
        /// </summary>
        /// <param name="client">The MongoClient.</param>
        /// <param name="databaseName">The name of your database.</param>
        public MongoDBContext(MongoClient client, string databaseName)
        {
            InitializeGuidRepresentation();
            InitializeConvention();
            Client = client;
            Database = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Initializes then current instance of the MongoDBContext.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string to use to connect to MongoDB.
        /// </param>
        /// <param name="databaseName">
        /// The name of the database to use in MongoDB.
        /// </param>
        public void InitializeConnection(string connectionString, string databaseName)
        {
            InitializeConvention();
            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(databaseName);
        }
        /// <summary>
        /// Returns a collection for a document type.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        public virtual IMongoCollection<TDocument> GetCollection<TDocument>()
        where TDocument : class
        => Database.GetCollection<TDocument>(GetCollectionName<TDocument>());
        /// <summary>
        /// Given the document type and the partition key, returns the name of the collection it belongs to.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The value of the partition key.</param>
        /// <returns>The name of the collection.</returns>
        protected virtual string GetCollectionName<TDocument>(string partitionKey = null)
        where TDocument : class
        {
            string collectionName = GetAttributeCollectionName<TDocument>() ?? Pluralize<TDocument>();
            if (string.IsNullOrEmpty(partitionKey))
            {
                return collectionName;
            }
            return $"{partitionKey}-{collectionName}";
        }
        /// <summary>
        /// Returns a collection for a document type. Also handles document types with a partition key.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The optional value of the partition key.</param>
        public virtual IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null)
        where TDocument : class
        {
            return Database.GetCollection<TDocument>(GetCollectionName<TDocument>(partitionKey));
        }

        /// <summary>
        /// Drops a collection, use very carefully.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        public virtual void DropCollection<TDocument>()
        where TDocument : class
        => Database.DropCollection(GetCollectionName<TDocument>());
        /// <summary>
        /// Drops a collection based on the specified name.
        /// </summary>
        public virtual void DropCollection(string name) => Database.DropCollection(name);
        /// <summary>
        /// Drops the database based on the specified name. Use wisely.
        /// </summary>
        public virtual void DropDatabase(string name) => Client.DropDatabase(name);
        /// <summary>
        /// Drops a database, use very very carefully.
        /// </summary>
        public virtual async Task DropDatabaseAsync(string name) => await Client.DropDatabaseAsync(name);

        /// <summary>
        /// Initialize the Guid representation of the MongoDB Driver.
        /// Override this method to change the default GuidRepresentation.
        /// </summary>
        protected virtual void InitializeGuidRepresentation()
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V2;
        }
        /// <summary>
        /// Extracts the CollectionName attribute from the entity type, if any.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The name of the collection in which the TDocument is stored.</returns>
        /// <summary>
        /// Extracts the CollectionName attribute from the entity type, if any.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The name of the collection in which the TDocument is stored.</returns>
        protected virtual string GetAttributeCollectionName<TDocument>()
        => (typeof(TDocument).GetTypeInfo()
                             .GetCustomAttributes(typeof(CollectionNameAttribute))
                             .FirstOrDefault() as CollectionNameAttribute)?.Name;

        /// <summary>
        /// Initialize the Guid representation of the MongoDB Driver.
        /// Override this method to change the default GuidRepresentation.
        /// </summary>
        protected virtual void InitializeConvention()
        {
            ConventionPack conventionPack = new() { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register(DEFAULT_CONVENTION, conventionPack, (t) => true);
        }

        /// <summary>
        /// Given the document type and the partition key, returns the name of the collection it belongs to.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
	    /// <param name="partitionKey">The value of the partition key.</param>
        /// <returns>The name of the collection.</returns>
        protected virtual string GetCollectionName<TDocument>(string name, string partitionKey)
        where TDocument : class
        {
            var collectionName = GetAttributeCollectionName<TDocument>() ?? Pluralize<TDocument>();
            if (string.IsNullOrEmpty(partitionKey))
            {
                return collectionName;
            }
            return $"{partitionKey}-{collectionName}";
        }

        /// <summary>
        /// Very naively pluralizes a TDocument type name.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The pluralized document name.</returns>
        protected virtual string Pluralize<TDocument>()
        where TDocument : class
        => typeof(TDocument).Name.ToPlural().ToCamelCase();

    }
}

