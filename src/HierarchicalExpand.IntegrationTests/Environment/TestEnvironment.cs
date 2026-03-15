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

    protected IEnumerable<string> GetViews(string? schema)
    {
        yield return GetUndirectAncestorLinkTypeView(
            typeof(BusinessUnitUndirectAncestorLink),
            typeof(BusinessUnitDirectAncestorLink), schema);

        yield return GetUndirectAncestorLinkTypeView(
            typeof(TestHierarchicalObjectUndirectAncestorLink),
            typeof(TestHierarchicalObjectDirectAncestorLink),
            schema);
    }

    private static string GetUndirectAncestorLinkTypeView(Type undirectAncestorLinkType, Type directAncestorLinkType, string? schema)
    {
        var schemaPrefix = schema == null ? "" : $"{schema}_";

        return @$"
CREATE VIEW {schemaPrefix}{undirectAncestorLinkType.Name}
AS
    SELECT ancestorId as sourceId, childId as targetId, Id AS Id
    FROM {schemaPrefix}{directAncestorLinkType.Name}
UNION
    SELECT
         childId as sourceId, ancestorId as targetId, lower(
        substr(hex(Id), 17, 16) || '-' ||
        substr(hex(Id), 13, 4) || '-' ||
        substr(hex(Id), 9, 4) || '-' ||
        substr(hex(Id), 5, 4) || '-' ||
        substr(hex(Id), 1, 4) || substr(hex(Id), 21, 12)
    ) as Id
    FROM {schemaPrefix}{directAncestorLinkType.Name}";
    }
}