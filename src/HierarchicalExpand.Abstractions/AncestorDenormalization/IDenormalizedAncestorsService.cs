using CommonFramework;

namespace HierarchicalExpand.AncestorDenormalization;

public interface IDenormalizedAncestorsService : IInitializer;

public interface IDenormalizedAncestorsService<in TDomainObject> : IDenormalizedAncestorsService
{
    Task SyncUpAsync(TDomainObject domainObject, CancellationToken cancellationToken);

    Task SyncAsync(
        IEnumerable<TDomainObject> updatedDomainObjectsBase,
        IEnumerable<TDomainObject> removedDomainObjects,
        CancellationToken cancellationToken);
}