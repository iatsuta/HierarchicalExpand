using CommonFramework.GenericRepository;

using GenericQueryable;

namespace HierarchicalExpand;

public class DomainObjectExpander<TDomainObject>(HierarchicalInfo<TDomainObject> hierarchicalInfo, IQueryableSource queryableSource)
    : IDomainObjectExpander<TDomainObject>
    where TDomainObject : class
{
    private Dictionary<TDomainObject, TDomainObject?>? baseCache;

    public async ValueTask<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken)
    {
        var allResult = startDomainObjects.ToHashSet();

        for (var nextLayer = allResult; nextLayer.Any(); allResult.UnionWith(nextLayer))
        {
            nextLayer = nextLayer
                .Select(hierarchicalInfo.ParentFunc)
                .Where(nextObj => nextObj != null && !allResult.Contains(nextObj))
                .Select(nextObj => nextObj!)
                .ToHashSet();
        }

        return allResult;
    }

    public async ValueTask<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken)
    {
        var allResult = startDomainObjects.ToHashSet();

        if (allResult.Count != 0)
        {
            var cache = await this.GetCache(cancellationToken);

            for (var nextLayer = allResult; nextLayer.Any(); allResult.UnionWith(nextLayer))
            {
                nextLayer = cache.Where(pair => pair.Value != null && nextLayer.Contains(pair.Value))
                    .Select(pair => pair.Key)
                    .ToHashSet();
            }
        }

        return allResult;
    }

    private async ValueTask<IReadOnlyDictionary<TDomainObject, TDomainObject?>> GetCache(CancellationToken cancellationToken) =>
        this.baseCache ??= await queryableSource.GetQueryable<TDomainObject>().WithFetch(r => r.Fetch(hierarchicalInfo.ParentPath))
            .GenericToDictionaryAsync(d => d, hierarchicalInfo.ParentFunc, cancellationToken);
}