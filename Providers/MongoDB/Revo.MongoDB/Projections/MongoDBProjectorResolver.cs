using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Revo.Infrastructure.Projections;

namespace Revo.MongoDB.Projections
{
    public class MongoDBProjectorResolver : IMongoDBProjectorResolver
    {
        private readonly IKernel kernel;

        public MongoDBProjectorResolver(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public bool HasAnyProjectors(Type aggregateType)
        {
            var bindings = kernel.GetBindings(
                typeof(IMongoDBDocumentEventProjector<>).MakeGenericType(aggregateType));
            return bindings.Any();
        }

        public bool HasAnySyncProjectors(Type aggregateType)
        {
            var bindings = kernel.GetBindings(
                typeof(IMongoDBSyncDocumentEventProjector<>).MakeGenericType(aggregateType));
            return bindings.Any();
        }

        public IReadOnlyCollection<IEntityEventProjector> GetProjectors(Type aggregateType)
        {
            return kernel.GetAll(
                    typeof(IMongoDBDocumentEventProjector<>).MakeGenericType(aggregateType))
                .Cast<IEntityEventProjector>()
                .ToArray();
        }

        public IReadOnlyCollection<IEntityEventProjector> GetSyncProjectors(Type aggregateType)
        {
            return kernel.GetAll(
                    typeof(IMongoDBSyncDocumentEventProjector<>).MakeGenericType(aggregateType))
                .Cast<IEntityEventProjector>()
                .ToArray();
        }
    }
}
