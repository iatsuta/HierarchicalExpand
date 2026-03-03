using CommonFramework;

namespace HierarchicalExpand.Denormalization;

public interface IDeepLevelDenormalizer : IInitializer;

public interface IDeepLevelDenormalizer<in TDomainObject> : IDeepLevelDenormalizer
{
    Task UpdateDeepLevels(IEnumerable<TDomainObject> domainObjects, CancellationToken cancellationToken);
}