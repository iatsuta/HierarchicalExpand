namespace HierarchicalExpand.IntegrationTests.Domain;

public class TestHierarchicalObjectDirectAncestorLink
{
    public Guid Id { get; set; }

    public virtual required TestHierarchicalObject Ancestor { get; init; }

    public virtual required TestHierarchicalObject Child { get; init; }
}