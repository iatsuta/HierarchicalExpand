namespace HierarchicalExpand;

public interface IDomainObjectExpander<TDomainObject>
    where TDomainObject : class
{
    Task<HashSet<TDomainObject>> GetAllParents(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken);

    Task<HashSet<TDomainObject>> GetAllChildren(IEnumerable<TDomainObject> startDomainObjects, CancellationToken cancellationToken);
}