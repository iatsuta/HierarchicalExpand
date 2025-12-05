using HierarchicalExpand.AncestorDenormalization;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.DependencyInjection;

public class HierarchicalExpandSettings : IHierarchicalExpandSettings
{
	private readonly List<Action<IServiceCollection>> actions = new();

	public void Initialize(IServiceCollection services)
	{
		foreach (var action in this.actions)
		{
			action(services);
		}

		if (!this.AlreadyInitialized(services))
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
			services.AddSingleton(hierarchicalInfo);

			services.AddSingleton<FullAncestorLinkInfo>(fullAncestorLinkInfo);
			services.AddSingleton(fullAncestorLinkInfo);

			var directLinkType =
				typeof(FullAncestorLinkInfo<,>).MakeGenericType(fullAncestorLinkInfo.DomainObjectType, fullAncestorLinkInfo.DirectedLinkType);

			services.Add(ServiceDescriptor.Singleton(directLinkType, fullAncestorLinkInfo));
		});

		return this;
	}

	private IServiceCollection RegisterGeneralServices(IServiceCollection services)
	{
		return services
			.AddScoped(typeof(IDenormalizedAncestorsService<>), typeof(DenormalizedAncestorsService<>))
			.AddScoped(typeof(IAncestorLinkExtractor<,>), typeof(AncestorLinkExtractor<,>))
			.AddSingleton<IRealTypeResolver, IdentityRealTypeResolver>()
			.AddScoped<IHierarchicalObjectExpanderFactory, HierarchicalObjectExpanderFactory>()
			.AddScoped(typeof(IDomainObjectExpander<>), typeof(DomainObjectExpander<>))
			.AddSingleton<IHierarchicalInfoSource, HierarchicalInfoSource>();
	}

	private bool AlreadyInitialized(IServiceCollection services)
	{
		return services.Any(sd => !sd.IsKeyedService && sd.Lifetime == ServiceLifetime.Singleton && sd.ServiceType == typeof(IHierarchicalInfoSource));
	}
}