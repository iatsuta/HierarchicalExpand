namespace HierarchicalExpand;

public class IdentityActualTypeResolver : IActualDomainTypeResolver
{
    public Type Resolve(Type domainType)
    {
        return domainType;
    }
}