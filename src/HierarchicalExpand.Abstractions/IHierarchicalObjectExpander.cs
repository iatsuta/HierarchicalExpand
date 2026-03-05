using System.Collections;
using System.Linq.Expressions;

namespace HierarchicalExpand;

public interface IHierarchicalObjectExpander<TIdent> : IHierarchicalObjectExpander
    where TIdent : notnull
{
    IEnumerable<TIdent> Expand(IEnumerable<TIdent> idents, HierarchicalExpandType expandType);

    Expression<Func<IEnumerable<TIdent>, IEnumerable<TIdent>>> GetExpandExpression(HierarchicalExpandType expandType);

    Expression<Func<TIdent, IEnumerable<TIdent>>>? TryGetSingleExpandExpression(HierarchicalExpandType expandType);

    Dictionary<TIdent, TIdent> ExpandWithParents(IEnumerable<TIdent> idents, HierarchicalExpandType expandType);

    Dictionary<TIdent, TIdent> ExpandWithParents(IQueryable<TIdent> idents, HierarchicalExpandType expandType);

    IEnumerable IHierarchicalObjectExpander.Expand(IEnumerable idents, HierarchicalExpandType expandType) =>
        this.Expand((IEnumerable<TIdent>)idents, expandType);
}

public interface IHierarchicalObjectExpander
{
    IEnumerable Expand(IEnumerable idents, HierarchicalExpandType expandType);
}