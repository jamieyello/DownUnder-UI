using System;
using System.Linq.Expressions;
using System.Reflection;

// credit: https://stackoverflow.com/a/491486/7572353
namespace DownUnder.UI.Utilities
{
    public static class PropertyHelper<T>
    {
        public static PropertyInfo GetProperty<TValue>(
            Expression<Func<T, TValue>> selector)
        {
            Expression body = selector;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }
            return body.NodeType switch
            {
                ExpressionType.MemberAccess => (PropertyInfo)((MemberExpression)body).Member,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
