namespace HierarchicalExpand;

public interface IHierarchicalInfoSource
{
    HierarchicalInfo<TDomainObject> GetHierarchicalInfo<TDomainObject>();

    bool IsHierarchical(Type domainType);
}