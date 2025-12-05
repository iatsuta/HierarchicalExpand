namespace HierarchicalExpand.Tests.Domain;

public record DirectAncestorLink
{
	public required DomainObject From { get; init; }

	public required DomainObject To { get; init; }

	public override string ToString()
	{
		return $"{nameof(this.From)}:{this.From}|{nameof(this.To)}:{this.To}";
	}
}