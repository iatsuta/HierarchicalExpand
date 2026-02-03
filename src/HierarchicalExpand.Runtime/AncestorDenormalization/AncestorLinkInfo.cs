namespace HierarchicalExpand.AncestorDenormalization;

public record AncestorLinkInfo<TDomainObject>(TDomainObject Ancestor, TDomainObject Child);