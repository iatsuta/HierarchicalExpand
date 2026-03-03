using CommonFramework;

namespace HierarchicalExpand.Denormalization;

public interface IAncestorDenormalizer : IInitializer;

public interface IAncestorDenormalizer<in TDomainObject> : IAncestorDenormalizer
{
    Task SyncUpAsync(TDomainObject domainObject, CancellationToken cancellationToken);

    Task SyncAsync(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken cancellationToken);
}