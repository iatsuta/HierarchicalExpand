using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class TestHierarchicalObjectMap : ClassMap<TestHierarchicalObject>
{
    public TestHierarchicalObjectMap()
    {
        this.Schema("app");

        this.DynamicUpdate();

        this.Id(x => x.Id).GeneratedBy.GuidComb();
        this.Map(x => x.DeepLevel);

        this.References(x => x.Parent).Column(nameof(BusinessUnit.Parent) + "Id");
    }
}