namespace HierarchicalExpand.IntegrationTests.Domain;

public class TestHierarchicalObject
{
    public Guid Id { get; set; }

    public virtual TestHierarchicalObject? Parent { get; set; }

    public int DeepLevel { get; set; }
}