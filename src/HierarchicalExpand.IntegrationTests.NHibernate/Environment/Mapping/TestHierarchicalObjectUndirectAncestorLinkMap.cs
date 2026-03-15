using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class TestHierarchicalObjectUndirectAncestorLinkMap : ClassMap<TestHierarchicalObjectUndirectAncestorLink>
{
    public TestHierarchicalObjectUndirectAncestorLinkMap()
    {
        this.Schema("app");

        this.SchemaAction.None();

        this.Id(x => x.Id).GeneratedBy.Assigned();

        this.References(x => x.Source).Column("SourceId").Not.Nullable();
        this.References(x => x.Target).Column("TargetId").Not.Nullable();
    }
}