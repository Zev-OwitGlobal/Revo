using Revo.Infrastructure.Projections;

namespace Revo.MongoDB.Projections
{
    /// <summary>
    /// Represents a class for specifying <see cref="EventProjectionOptions"/> for MongoDB .
    /// driven projections.
    /// </summary>
    public class MongoDBEventProjectionOptions : EventProjectionOptions
    {
        /// <summary>
        /// Gets whether or not the projection is run synchronous or not.
        /// </summary>
        public bool IsSynchronousProjection { get; private set; }
        public MongoDBEventProjectionOptions(bool isSynchronousProjection)
        {
            IsSynchronousProjection = isSynchronousProjection;
        }
    }
}
