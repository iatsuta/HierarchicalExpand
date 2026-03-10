namespace HierarchicalExpand.IntegrationTests.Domain;

public class BusinessUnit
{
    public virtual Guid Id { get; set; }

    public virtual BusinessUnit? Parent { get; set; }

    public virtual required string Name { get; set; }

    public virtual int DeepLevel { get; set; }
}