namespace HierarchicalExpand.IntegrationTests.Domain;

public class BusinessUnitUndirectAncestorLink
{
    public virtual Guid FakeId { get; init; }

    public virtual required BusinessUnit Source { get; init; }

    public virtual required BusinessUnit Target { get; init; }
}