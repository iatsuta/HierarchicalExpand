using CommonFramework;

namespace HierarchicalExpand.Denormalization;

public interface IAncestorDenormalizer : IInitializer;

public interface IAncestorDenormalizer<in TDomainObject> : IAncestorDenormalizer
{
    Task SyncAsync(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken cancellationToken);
}