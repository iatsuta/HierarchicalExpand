using CommonFramework.DependencyInjection;
using CommonFramework.IdentitySource.DependencyInjection;

using HierarchicalExpand.Denormalization;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.DependencyInjection;

public class HierarchicalExpandSettings : IHierarchicalExpandSettings
{
    private readonly List<Action<IServiceCollection>> actions = new();

    public void Initialize(IServiceCollection services)
    {
        services.AddIdentitySource();

        foreach (var action in this.actions)
        {
            action(services);
        }

        if (!services.AlreadyInitialized<IHierarchicalInfoSource>())
        {
            this.RegisterGeneralServices(services);
        }
    }

    public IHierarchicalExpandSettings AddHierarchicalInfo<TDomainObject>(
        HierarchicalInfo<TDomainObject> hierarchicalInfo,
        FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo,
        DeepLevelInfo<TDomainObject>? deepLevelInfo = null)
    {
        this.actions.Add(services =>
        {
            services.AddSingleton<HierarchicalInfo>(hierarchicalInfo);
            services.AddSingleton(hierarchicalInfo);

            services.AddSingleton<FullAncestorLinkInfo>(fullAncestorLinkInfo);
            services.AddSingleton(fullAncestorLinkInfo);

            var directLinkType =
                typeof(FullAncestorLinkInfo<,>).MakeGenericType(fullAncestorLinkInfo.DomainObjectType, fullAncestorLinkInfo.DirectedLinkType);

            services.AddSingleton(directLinkType, fullAncestorLinkInfo);

            if (deepLevelInfo != null)
            {
                services.AddSingleton<DeepLevelInfo>(deepLevelInfo);
                services.AddSingleton(deepLevelInfo);
            }
        });

        return this;
    }

    private IServiceCollection RegisterGeneralServices(IServiceCollection services)
    {
        return services
            .AddServiceProxyFactory()
            .AddScoped<IDeepLevelDenormalizer, DeepLevelDenormalizer>()
            .AddScoped(typeof(IDeepLevelDenormalizer<>), typeof(DeepLevelDenormalizer<>))
            .AddScoped<IAncestorDenormalizer, AncestorDenormalizer>()
            .AddScoped(typeof(IAncestorDenormalizer<>), typeof(AncestorDenormalizer<>))
            .AddScoped(typeof(IAncestorLinkExtractor<,>), typeof(AncestorLinkExtractor<,>))
            .AddSingleton<IRealTypeResolver, IdentityRealTypeResolver>()
            .AddScoped<IHierarchicalObjectExpanderFactory, HierarchicalObjectExpanderFactory>()
            .AddScoped(typeof(IDomainObjectExpanderFactory<>), typeof(DomainObjectExpanderFactory<>))
            .AddSingleton<IHierarchicalInfoSource, HierarchicalInfoSource>();
    }
}