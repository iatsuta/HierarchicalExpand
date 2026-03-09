namespace HierarchicalExpand.IntegrationTests.Domain;

public class TestHierarchicalObject
{
    public virtual Guid Id { get; set; }

    public virtual TestHierarchicalObject? Parent { get; set; }

    public virtual int DeepLevel { get; set; }
}