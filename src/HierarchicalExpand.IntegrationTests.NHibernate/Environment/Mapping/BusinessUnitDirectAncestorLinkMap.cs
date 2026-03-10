using FluentNHibernate.Mapping;

using HierarchicalExpand.IntegrationTests.Domain;

namespace HierarchicalExpand.IntegrationTests.Environment.Mapping;

public class BusinessUnitDirectAncestorLinkMap : ClassMap<BusinessUnitDirectAncestorLink>
{
    public BusinessUnitDirectAncestorLinkMap()
    {
        this.Schema("app");

        this.DynamicUpdate();

        this.Id(x => x.Id).GeneratedBy.GuidComb();

        this.References(x => x.Ancestor).Column(nameof(BusinessUnitDirectAncestorLink.Ancestor) + "Id").Not.Nullable();
        this.References(x => x.Child).Column(nameof(BusinessUnitDirectAncestorLink.Child) + "Id").Not.Nullable();
    }
}