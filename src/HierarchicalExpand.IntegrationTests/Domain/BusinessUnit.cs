namespace HierarchicalExpand.IntegrationTests.Domain;

public class BusinessUnit
{
    public Guid Id { get; set; }

    public virtual BusinessUnit? Parent { get; set; }

    public required string Name { get; set; }
}