using System.Collections.Concurrent;

using CommonFramework.IdentitySource;

namespace HierarchicalExpand;

public class HierarchicalObjectExpanderTypeResolver(
    IServiceProvider serviceProvider,
    IIdentityInfoSource identityInfoSource,
    IActualDomainTypeResolver realTypeResolver) : IHierarchicalObjectExpanderTypeResolver
{
    private readonly ConcurrentDictionary<Type, Type> cache = [];

    public Type Resolve(Type domainType)
    {
        return this.cache.GetOrAdd(domainType, _ =>
        {
            var realDomainType = realTypeResolver.Resolve(domainType);

            if (realDomainType != domainType)
            {
                return this.Resolve(realDomainType);
            }
            else
            {
                var fullAncestorLinkInfo = (FullAncestorLinkInfo?)serviceProvider.GetService(typeof(FullAncestorLinkInfo<>).MakeGenericType(domainType));

                var identityInfo = identityInfoSource.GetIdentityInfo(domainType);

                if (fullAncestorLinkInfo != null)
                {
                    return typeof(HierarchicalObjectAncestorLinkExpander<,,,>)
                        .MakeGenericType(domainType, fullAncestorLinkInfo.DirectedLinkType, fullAncestorLinkInfo.UndirectedLinkType, identityInfo.IdentityType);
                }
                else
                {
                    return typeof(PlainHierarchicalObjectExpander<>).MakeGenericType(identityInfo.IdentityType);
                }
            }
        });
    }
}