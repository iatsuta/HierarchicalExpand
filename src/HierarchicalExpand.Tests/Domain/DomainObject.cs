namespace HierarchicalExpand.Tests.Domain;

public class DomainObject
{
	public required string Name { get; init; }

	public DomainObject? Parent { get; set; }

	public int Id { get; init; }

	public override string ToString() => this.Name;
}