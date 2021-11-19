namespace Revo.MongoDB.EventStores
{
    public class MongoDBEventStoreSettings
    {
        public string EventStreamRowCollectionName { get; set; } = "EventStreamRows";
        public string EventStreamCollectionName { get; set; } = "EventStreams";
        public string DatabaseName { get; set; } = "EventStore";
    }
}
