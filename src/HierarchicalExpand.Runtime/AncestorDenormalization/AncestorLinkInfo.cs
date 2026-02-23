namespace HierarchicalExpand.AncestorDenormalization;

public record AncestorLinkData<TDomainObject>(TDomainObject Ancestor, TDomainObject Child);