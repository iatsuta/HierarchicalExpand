namespace HierarchicalExpand;

public interface IDomainObjectExpander<TDomainObject>
    where TDomainObject : class
{
    ValueTask<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken);

    ValueTask<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken);
}