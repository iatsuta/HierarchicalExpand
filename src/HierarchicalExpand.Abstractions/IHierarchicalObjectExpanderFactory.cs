namespace HierarchicalExpand;

public interface IHierarchicalObjectExpanderFactory
{
    IHierarchicalObjectExpander Create(Type domainType);
}