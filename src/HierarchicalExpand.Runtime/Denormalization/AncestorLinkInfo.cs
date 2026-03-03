namespace HierarchicalExpand.Denormalization;

public record AncestorLinkData<TDomainObject>(TDomainObject Ancestor, TDomainObject Child);