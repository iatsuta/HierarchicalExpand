using CommonFramework;

namespace HierarchicalExpand;

public class DomainObjectExpanderFactory<TDomainObject>(IServiceProxyFactory serviceProxyFactory) : IDomainObjectExpanderFactory<TDomainObject>
    where TDomainObject : class
{
    public IDomainObjectExpander<TDomainObject> Create(bool cached = true)
    {
        return serviceProxyFactory.Create<IDomainObjectExpander<TDomainObject>, DomainObjectExpander<TDomainObject>>(cached);
    }
}