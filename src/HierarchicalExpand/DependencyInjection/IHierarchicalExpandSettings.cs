using System.Linq.Expressions;

namespace HierarchicalExpand.DependencyInjection;

public interface IHierarchicalExpandSettings
{
	IHierarchicalExpandSettings AddHierarchicalInfo<TDomainObject>(
		HierarchicalInfo<TDomainObject> hierarchicalInfo,
		FullAncestorLinkInfo<TDomainObject> fullAncestorLinkInfo);

	IHierarchicalExpandSettings AddHierarchicalInfo<TDomainObject, TDirectedLink, TUndirectedLink>(
		Expression<Func<TDomainObject, TDomainObject?>> parentPath,
		AncestorLinkInfo<TDomainObject, TDirectedLink> directed,
		AncestorLinkInfo<TDomainObject, TUndirectedLink> undirected) =>
		this.AddHierarchicalInfo(
			new HierarchicalInfo<TDomainObject>(parentPath),
			new FullAncestorLinkInfo<TDomainObject, TDirectedLink, TUndirectedLink>(directed, undirected));
}