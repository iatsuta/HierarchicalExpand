namespace HierarchicalExpand;

public interface IDomainObjectExpanderFactory<TDomainObject>
    where TDomainObject : class
{
    IDomainObjectExpander<TDomainObject> Create(bool cached = true);
}