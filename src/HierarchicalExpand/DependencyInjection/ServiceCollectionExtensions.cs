using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddHierarchicalExpand(Action<IHierarchicalExpandBuilder>? setupAction = null) =>
            services.Initialize<HierarchicalExpandBuilder>(setupAction);
    }
}