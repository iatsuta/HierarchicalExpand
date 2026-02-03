using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand;

public class HierarchicalInfoSource(IServiceProvider serviceProvider) : IHierarchicalInfoSource
{
    public HierarchicalInfo<TDomainObject> GetHierarchicalInfo<TDomainObject>()
    {
        return serviceProvider.GetRequiredService<HierarchicalInfo<TDomainObject>>();
    }

    public bool IsHierarchical(Type domainType)
    {
        return serviceProvider.GetService(typeof(HierarchicalInfo<>).MakeGenericType(domainType)) != null;
    }
}