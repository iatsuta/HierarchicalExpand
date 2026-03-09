using CommonFramework;
using CommonFramework.DependencyInjection;

using HierarchicalExpand.DependencyInjection;
using HierarchicalExpand.IntegrationTests.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.IntegrationTests.Environment;

public abstract class TestEnvironment
{
    public IServiceProvider RootServiceProvider => field ??= BuildServiceProvider();

    protected IServiceProvider BuildServiceProvider()
    {
        return new ServiceCollection()
            .Pipe(this.InitializeServices)

            .AddHierarchicalExpand(scb => scb
                .AddHierarchicalInfo(
                    v => v.Parent,
                    new AncestorLinkInfo<BusinessUnit, BusinessUnitDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                    new AncestorLinkInfo<BusinessUnit, BusinessUnitUndirectAncestorLink>(view => view.Source, view => view.Target),
                    v => v.DeepLevel)
                .AddHierarchicalInfo(
                    v => v.Parent,
                    new AncestorLinkInfo<TestHierarchicalObject, TestHierarchicalObjectDirectAncestorLink>(link => link.Ancestor, link => link.Child),
                    new AncestorLinkInfo<TestHierarchicalObject, TestHierarchicalObjectUndirectAncestorLink>(view => view.Source, view => view.Target),
                    v => v.DeepLevel))

            .AddSingleton<ScopeEvaluator>()
            .AddSingleton<TestDataInitializer>()
            .AddValidator<DuplicateServiceUsageValidator>()
            .Validate()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    protected abstract IServiceCollection InitializeServices(IServiceCollection services);

    public abstract Task InitializeDatabase();

    protected IEnumerable<string> GetViews()
    {
        yield return @$"
CREATE VIEW {nameof(BusinessUnitUndirectAncestorLink)}
AS
    SELECT ancestorId as sourceId, childId as targetId
FROM {nameof(BusinessUnitDirectAncestorLink)}
UNION
    SELECT childId as sourceId, ancestorId as targetId
FROM {nameof(BusinessUnitDirectAncestorLink)}
";

        yield return @$"
CREATE VIEW {nameof(TestHierarchicalObjectUndirectAncestorLink)}
AS
SELECT ancestorId as sourceId, childId as targetId
FROM {nameof(TestHierarchicalObjectDirectAncestorLink)}
UNION
SELECT childId as sourceId, ancestorId as targetId
FROM {nameof(TestHierarchicalObjectDirectAncestorLink)}
";
    }
}