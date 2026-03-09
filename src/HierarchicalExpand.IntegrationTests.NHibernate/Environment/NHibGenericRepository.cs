using CommonFramework;
using CommonFramework.GenericRepository;
using CommonFramework.IdentitySource;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class NHibGenericRepository(
    AutoCommitSession session,
    IIdentityInfoSource identityInfoSource) : IGenericRepository
{
    public Task SaveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TDomainObject>();

        return new Func<TDomainObject, CancellationToken, Task>(this.SaveAsync<TDomainObject, Ignore>)
            .CreateGenericMethod(identityInfo.DomainObjectType, identityInfo.IdentityType)
            .Invoke<Task>(this, domainObject, cancellationToken);
    }

    public async Task SaveAsync<TDomainObject, TIdent>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class
        where TIdent : notnull
    {
        if (!session.NativeSession.Contains(domainObject))
        {
            var identityInfo = identityInfoSource.GetIdentityInfo<TDomainObject, TIdent>();

            var id = identityInfo.Id.Getter(domainObject);

            if (!EqualityComparer<TIdent>.Default.Equals(id, default))
            {
                await session.NativeSession.SaveAsync(domainObject, id, cancellationToken);

                return;
            }
        }

        await session.NativeSession.SaveOrUpdateAsync(domainObject, cancellationToken);
    }

    public Task RemoveAsync<TDomainObject>(TDomainObject domainObject, CancellationToken cancellationToken)
        where TDomainObject : class => session.NativeSession.DeleteAsync(domainObject, cancellationToken);
}