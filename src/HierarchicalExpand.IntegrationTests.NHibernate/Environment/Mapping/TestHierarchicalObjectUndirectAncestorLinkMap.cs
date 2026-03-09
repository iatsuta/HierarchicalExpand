using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class TestHierarchicalObjectUndirectAncestorLinkMap : ClassMap<TestHierarchicalObjectUndirectAncestorLink>
{
    public TestHierarchicalObjectUndirectAncestorLinkMap()
    {
        this.Schema("app");

        this.Id(x => x.FakeId).GeneratedBy.GuidComb();

        this.References(x => x.Source).Column("SourceId").Not.Nullable();
        this.References(x => x.Target).Column("TargetId").Not.Nullable();
    }
}