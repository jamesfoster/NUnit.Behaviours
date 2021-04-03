using NUnit.Framework.Internal;

using System;
using System.Collections.Generic;
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
            );

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

        private string GetExpressionText(Expression expression)
        {
            return ExpressionPrinter.Print(expression);
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
