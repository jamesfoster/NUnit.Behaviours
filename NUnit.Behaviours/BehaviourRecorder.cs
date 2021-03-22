using NUnit.Framework.Internal;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace NUnit.Behaviours
{
    internal class BehaviourRecorder : Behaviour
    {
        public IList<string> GivenNames = Array.Empty<string>();
        public IList<string> WhenNames = Array.Empty<string>();
        public IList<string> ThenNames = Array.Empty<string>();

        public string Name(int index) => string.Join(
                ", ",
                GivenNames
                    .Concat(WhenNames)
                    .Concat(new[] { ThenNames[index] })
            ).Replace("_", " ");

        public int Length => ThenNames.Count;

        public void Given(params Expression<Action>[] actions)
        {
            GivenNames = actions.Select(GetExpressionText).ToList();
        }

        public void When(params Expression<Action>[] actions)
        {
            WhenNames = actions.Select(GetExpressionText).ToList();
        }

        public void Then(params Expression<Action>[] actions)
        {
            ThenNames = actions.Select(GetExpressionText).ToList();
        }

        private string GetExpressionText(Expression action)
        {
            return action switch
            {
                LambdaExpression lambda => GetExpressionText(lambda.Body),
                MethodCallExpression call => GetMethodCallText(call),
                MemberExpression member => member.Member.Name,
                UnaryExpression unary when unary.NodeType == ExpressionType.Convert => GetExpressionText(unary.Operand),
                _ => action.ToString()
            };
        }

        private string GetMethodCallText(MethodCallExpression call)
        {
            var result = $"{call.Method.Name}{GetArgumentsText(call.Arguments)}";

            if (call.Object == null)
                return $"{call.Method.DeclaringType?.Name}.{result}";

            return result;
        }

        private string GetArgumentsText(ReadOnlyCollection<Expression> arguments)
        {
            if (arguments.Count == 0) return string.Empty;

            return "(" + string.Join(", ", arguments.Select(GetExpressionText)) + ")";
        }

        internal TestCaseParameters GetTestPamameters(int i, InitializationLock locker)
        {
            var behaviour = new BehaviourExecutor(i, locker);

            var parms = new TestCaseParameters(new[] { behaviour })
            {
                TestName = Name(i)
            };

            return parms;
        }
    }
}
