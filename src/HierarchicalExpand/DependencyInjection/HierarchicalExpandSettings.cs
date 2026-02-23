using CommonFramework.DependencyInjection;
using CommonFramework.IdentitySource.DependencyInjection;

using HierarchicalExpand.AncestorDenormalization;

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
        FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo)
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
        });

        return this;
    }

    private IServiceCollection RegisterGeneralServices(IServiceCollection services)
    {
        return services
            .AddServiceProxyFactory()
            .AddScoped<IDenormalizedAncestorsService, DenormalizedAncestorsService>()
            .AddScoped(typeof(IDenormalizedAncestorsService<>), typeof(DenormalizedAncestorsService<>))
            .AddScoped(typeof(IAncestorLinkExtractor<,>), typeof(AncestorLinkExtractor<,>))
            .AddSingleton<IRealTypeResolver, IdentityRealTypeResolver>()
            .AddScoped<IHierarchicalObjectExpanderFactory, HierarchicalObjectExpanderFactory>()
            .AddScoped(typeof(IDomainObjectExpanderFactory<>), typeof(DomainObjectExpanderFactory<>))
            .AddSingleton<IHierarchicalInfoSource, HierarchicalInfoSource>();
    }
}