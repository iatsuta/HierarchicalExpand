using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class BusinessUnitMap : ClassMap<BusinessUnit>
{
    public BusinessUnitMap()
    {
        this.Schema("app");

        this.DynamicUpdate();

        this.Id(x => x.Id).GeneratedBy.GuidComb();

        this.Map(x => x.Name).Not.Nullable();

        this.Map(x => x.DeepLevel);

        this.References(x => x.Parent).Column(nameof(BusinessUnit.Parent) + "Id");
    }
}