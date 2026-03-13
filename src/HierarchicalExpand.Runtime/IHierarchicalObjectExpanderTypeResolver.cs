namespace HierarchicalExpand;

public interface IHierarchicalObjectExpanderTypeResolver
{
    Type Resolve(Type domainType);
}