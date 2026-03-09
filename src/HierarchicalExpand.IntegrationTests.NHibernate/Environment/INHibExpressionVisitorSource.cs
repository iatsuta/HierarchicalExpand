using System.Linq.Expressions;

namespace HierarchicalExpand.IntegrationTests.Environment;

public interface INHibExpressionVisitorSource
{
    ExpressionVisitor Visitor { get; }
}