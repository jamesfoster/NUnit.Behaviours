
using System;
using System.Linq.Expressions;

namespace NUnit.Behaviours
{
    internal class BehaviourExecutor : Behaviour
    {
        private readonly int index;
        private readonly InitializationLock locker;

        private Expression<Action>[] given = Array.Empty<Expression<Action>>();
        private Expression<Action>[] when = Array.Empty<Expression<Action>>();

        public BehaviourExecutor(int index, InitializationLock locker)
        {
            this.index = index;
            this.locker = locker;
        }

        public void Given(params Expression<Action>[] actions)
        {
            given = actions;
        }

        public void When(params Expression<Action>[] actions)
        {
            when = actions;
        }

        public void Then(params Expression<Action>[] actions)
        {
            var target = given.Length > 0 ? GetTarget(given[0]) : GetTarget(when[0]);

            locker.Run(target, () =>
            {
                foreach (var action in given)
                    action.Compile().Invoke();

                foreach (var action in when)
                    action.Compile().Invoke();
            });

            actions[index].Compile().Invoke();
        }

        private object? GetTarget(Expression<Action> expression)
        {
            if (expression is LambdaExpression lambda)
                if (lambda.Body is MethodCallExpression methodCall)
                    if (methodCall.Object is ConstantExpression constant)
                    {
                        return constant.Value;
                    }

            return null;
        }
    }
}
