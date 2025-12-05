using CommonFramework.IdentitySource.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.DependencyInjection;

public static class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddHierarchicalExpand(Action<IHierarchicalExpandSettings>? setupAction = null)
		{
			services.AddIdentitySource();

			var settings = new HierarchicalExpandSettings();

			setupAction?.Invoke(settings);

			settings.Initialize(services);

			return services;
		}
	}
}