using System;
using System.Collections.Generic;

using Revo.Infrastructure.Projections;

namespace Revo.MongoDB.Projections
{
    public interface IMongoDBProjectorResolver
    {
        IReadOnlyCollection<IEntityEventProjector> GetProjectors(Type aggregateType);
        IReadOnlyCollection<IEntityEventProjector> GetSyncProjectors(Type aggregateType);
        bool HasAnyProjectors(Type aggregateType);
    }
}
