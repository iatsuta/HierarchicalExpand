using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class BusinessUnitUndirectAncestorLinkMap : ClassMap<BusinessUnitUndirectAncestorLink>
{
    public BusinessUnitUndirectAncestorLinkMap()
    {
        this.Schema("app");

        this.Id(x => x.FakeId).GeneratedBy.GuidComb();

        this.References(x => x.Source).Column(nameof(BusinessUnitUndirectAncestorLink.Source) + "Id").Not.Nullable();
        this.References(x => x.Target).Column(nameof(BusinessUnitUndirectAncestorLink.Target) + "Id").Not.Nullable();
    }
}