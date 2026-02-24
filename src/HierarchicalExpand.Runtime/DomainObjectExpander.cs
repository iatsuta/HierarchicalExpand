using CommonFramework.GenericRepository;

using GenericQueryable;

namespace HierarchicalExpand;

public class DomainObjectExpander<TDomainObject>(HierarchicalInfo<TDomainObject> hierarchicalInfo, IQueryableSource queryableSource, bool cached)
	: IDomainObjectExpander<TDomainObject>
	where TDomainObject : class
{
    private Dictionary<TDomainObject, TDomainObject?>? baseCache;

    public async Task<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken)
    {
        var cache = await this.GetCache(cancellationToken);

        var allResult = startDomainObjects.ToHashSet();

        for (var nextLayer = allResult; nextLayer.Any(); allResult.UnionWith(nextLayer))
        {
            nextLayer = cache.Where(pair => nextLayer.Contains(pair.Key) && pair.Value != null)
                .Select(pair => pair.Value!)
                .ToHashSet();
        }

        return allResult;
    }

    public async Task<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken)
    {
        var cache = await this.GetCache(cancellationToken);

        var allResult = startDomainObjects.ToHashSet();

        for (var nextLayer = allResult; nextLayer.Any(); allResult.UnionWith(nextLayer))
        {
            nextLayer = cache.Where(pair => pair.Value != null && nextLayer.Contains(pair.Value))
                .Select(pair => pair.Key)
                .ToHashSet();
        }

        return allResult;
    }

    private async Task<IReadOnlyDictionary<TDomainObject, TDomainObject?>> GetCache(CancellationToken cancellationToken) =>
        cached ? this.baseCache ??= await this.CalcCacheValue(cancellationToken) : await this.CalcCacheValue(cancellationToken);

    private async Task<Dictionary<TDomainObject, TDomainObject?>> CalcCacheValue(CancellationToken cancellationToken) =>
        await queryableSource.GetQueryable<TDomainObject>().WithFetch(r => r.Fetch(hierarchicalInfo.ParentPath))
            .GenericToDictionaryAsync(d => d, hierarchicalInfo.ParentFunc, cancellationToken);
}