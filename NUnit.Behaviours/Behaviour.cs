
using System;
using System.Linq.Expressions;

namespace NUnit.Behaviours
{
    public interface Behaviour
    {
        void Given(params Expression<Action>[] actions);
        void When(params Expression<Action>[] actions);
        void Then(params Expression<Action>[] actions);
    }

    public static class BehaviourExtensions
    {
        public static void Arrange(this Behaviour b, params Expression<Action>[] actions) => b.Given(actions);
        public static void Act(this Behaviour b, params Expression<Action>[] actions) => b.When(actions);
        public static void Assert(this Behaviour b, params Expression<Action>[] actions) => b.Then(actions);
    }
}
