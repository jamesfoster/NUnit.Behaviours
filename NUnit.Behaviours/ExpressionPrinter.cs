using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace NUnit.Behaviours
{
    internal class ExpressionPrinter
    {
        internal static string Print(Expression expression)
        {
            return expression switch
            {
                LambdaExpression lambda => Print(lambda.Body),
                MethodCallExpression call => GetMethodCallText(call),
                MemberExpression member => member.Member.Name,
                UnaryExpression unary when unary.NodeType == ExpressionType.Convert => Print(unary.Operand),
                _ => expression.ToString()
            };
        }

        private static string GetMethodCallText(MethodCallExpression call)
        {
            var methodName = call.Method.Name.Replace("_", " ");
            var genericArgs = GetGenericArguments(call);
            var args = GetArgumentsText(call.Arguments);

            var result = $"{methodName}{genericArgs}{args}";

            if (call.Object == null)
                result = $"{call.Method.DeclaringType?.Name.Replace("_", " ")}.{result}";

            return result;
        }

        private static string GetGenericArguments(MethodCallExpression call)
        {
            var genericArgs = call.Method.GetGenericArguments();

            if (genericArgs.Length == 0) return string.Empty;

            return $"<{string.Join(", ", genericArgs.Select(x => x.Name))}>";
        }

        private static string GetArgumentsText(ReadOnlyCollection<Expression> arguments)
        {
            if (arguments.Count == 0) return string.Empty;

            return "(" + string.Join(", ", arguments.Select(Print)) + ")";
        }

    }
}
