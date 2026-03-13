using CommonFramework;

using System.Collections.Concurrent;

namespace HierarchicalExpand;

public class HierarchicalObjectExpanderFactory(
    IServiceProxyFactory serviceProxyFactory,
    IHierarchicalObjectExpanderTypeResolver hierarchicalObjectExpanderTypeResolver)
    : IHierarchicalObjectExpanderFactory
{
    private readonly ConcurrentDictionary<Type, IHierarchicalObjectExpander> cache = [];

    public IHierarchicalObjectExpander Create(Type domainType) =>
        cache.GetOrAdd(domainType, _ => serviceProxyFactory.Create<IHierarchicalObjectExpander>(hierarchicalObjectExpanderTypeResolver.Resolve(domainType)));
}