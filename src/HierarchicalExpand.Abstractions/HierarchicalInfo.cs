using System.Linq.Expressions;

namespace HierarchicalExpand;

public record HierarchicalInfo<TDomainObject>(Expression<Func<TDomainObject, TDomainObject?>> ParentPath)
{
    public Func<TDomainObject, TDomainObject?> ParentFunc { get; } = ParentPath.Compile();
}