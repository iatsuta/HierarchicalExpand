namespace HierarchicalExpand;

public interface IActualDomainTypeResolver
{
    Type Resolve(Type domainType);
}