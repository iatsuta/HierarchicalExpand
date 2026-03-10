namespace HierarchicalExpand.IntegrationTests.Domain;

public class TestHierarchicalObjectUndirectAncestorLink
{
    public virtual Guid FakeId { get; init; }

    public virtual required TestHierarchicalObject Source { get; init; }

    public virtual required TestHierarchicalObject Target { get; init; }
}