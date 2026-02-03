using CommonFramework;
using CommonFramework.DictionaryCache;
using CommonFramework.IdentitySource;

namespace HierarchicalExpand;

public class HierarchicalObjectExpanderFactory : IHierarchicalObjectExpanderFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly IServiceProxyFactory serviceProxyFactory;
    private readonly IIdentityInfoSource identityInfoSource;
    private readonly IRealTypeResolver realTypeResolver;
    private readonly IDictionaryCache<Type, IHierarchicalObjectExpander> cache;

    public HierarchicalObjectExpanderFactory(IServiceProvider serviceProvider, IServiceProxyFactory serviceProxyFactory, IIdentityInfoSource identityInfoSource,
        IRealTypeResolver realTypeResolver)
    {
        this.serviceProvider = serviceProvider;
        this.serviceProxyFactory = serviceProxyFactory;
        this.identityInfoSource = identityInfoSource;
        this.realTypeResolver = realTypeResolver;

        this.cache = new DictionaryCache<Type, IHierarchicalObjectExpander>(this.CreateInternal);
    }

    private IHierarchicalObjectExpander CreateInternal(Type domainType)
    {
        var realType = realTypeResolver.Resolve(domainType);

        if (realType != domainType)
        {
            return this.cache[realType];
        }
        else
        {
            var fullAncestorLinkInfo = (FullAncestorLinkInfo?)serviceProvider.GetService(typeof(FullAncestorLinkInfo<>).MakeGenericType(domainType));

            var identityInfo = identityInfoSource.GetIdentityInfo(domainType);

            var (serviceType, args) = fullAncestorLinkInfo != null
                ? (typeof(HierarchicalObjectAncestorLinkExpander<,,,>)
                        .MakeGenericType(domainType, fullAncestorLinkInfo.DirectedLinkType, fullAncestorLinkInfo.UndirectedLinkType, identityInfo.IdentityType),
                    [fullAncestorLinkInfo, identityInfo])

                : (typeof(PlainHierarchicalObjectExpander<>).MakeGenericType(identityInfo.IdentityType), Array.Empty<object>());

            return serviceProxyFactory.Create<IHierarchicalObjectExpander>(serviceType, args);
        }
    }

    public IHierarchicalObjectExpander Create(Type domainType)
    {
        return this.cache[domainType];
    }
}