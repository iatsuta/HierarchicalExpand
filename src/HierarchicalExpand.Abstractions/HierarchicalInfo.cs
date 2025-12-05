using System.Linq.Expressions;

namespace HierarchicalExpand;

public record HierarchicalInfo<TDomainObject>(Expression<Func<TDomainObject, TDomainObject?>> ParentPath) : HierarchicalInfo
{
	public Func<TDomainObject, TDomainObject?> ParentFunc { get; } = ParentPath.Compile();

	public override Type DomainObjectType { get; } = typeof(TDomainObject);
}

public abstract record HierarchicalInfo
{
	public abstract Type DomainObjectType { get; }
}