using CommonFramework.GenericRepository;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class EfQueryableSource(AutoCommitSession session) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        return session.NativeSession.Set<TDomainObject>();
    }
}