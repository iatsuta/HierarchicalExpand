namespace HierarchicalExpand;

public interface IDomainObjectExpanderFactory<TDomainObject>
    where TDomainObject : class
{
    IDomainObjectExpander<TDomainObject> Create();
}