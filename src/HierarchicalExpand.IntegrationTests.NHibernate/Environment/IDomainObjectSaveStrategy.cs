using NHibernate;

namespace HierarchicalExpand.IntegrationTests.Environment;

public interface IDomainObjectSaveStrategy<in TDomainObject>
{
    Task SaveAsync(ISession session, TDomainObject domainObject, CancellationToken cancellationToken);
}