using CommonFramework.GenericRepository;

using Microsoft.EntityFrameworkCore;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class EfGenericRepository(AutoCommitSession session) : IGenericRepository
{
    public async Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class
    {
        var state = session.NativeSession.Entry(domainObject).State;

        if (state == EntityState.Detached)
        {
            await session.NativeSession.AddAsync(domainObject, cancellationToken);
        }
    }

    public async Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class
    {
        session.NativeSession.Remove(domainObject);
    }
}