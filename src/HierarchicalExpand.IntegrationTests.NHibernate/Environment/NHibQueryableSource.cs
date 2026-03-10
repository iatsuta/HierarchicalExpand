using CommonFramework.GenericRepository;

using GenericQueryable.NHibernate;
using GenericQueryable.Services;

namespace HierarchicalExpand.IntegrationTests.Environment;

public class NHibQueryableSource(
    AutoCommitSession session,
    IGenericQueryableExecutor genericQueryableExecutor,
    INHibExpressionVisitorSource? nhibExpressionVisitorSource = null) : IQueryableSource
{
    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        var queryable = session.NativeSession.Query<TDomainObject>();

        var queryProvider = queryable.Provider as VisitedNHibQueryProvider ??
                            throw new InvalidOperationException($"Register {nameof(VisitedNHibQueryProvider)} in Nhib configuration");

        queryProvider.Visitor = nhibExpressionVisitorSource?.Visitor;
        queryProvider.Executor = genericQueryableExecutor;

        return queryable;
    }
}