using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class TestHierarchicalObjectDirectAncestorLinkMap : ClassMap<TestHierarchicalObjectDirectAncestorLink>
{
    public TestHierarchicalObjectDirectAncestorLinkMap()
    {
        this.Schema("app");

        this.DynamicUpdate();

        this.Id(x => x.Id).GeneratedBy.GuidComb();

        this.References(x => x.Ancestor).Column(nameof(TestHierarchicalObjectDirectAncestorLink.Ancestor) + "Id").Not.Nullable();
        this.References(x => x.Child).Column(nameof(TestHierarchicalObjectDirectAncestorLink.Child) + "Id").Not.Nullable();
    }
}