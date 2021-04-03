using NUnit.Framework;

namespace NUnit.Behaviours.TestFixtures
{
    [BehaviourFixture]
    public class TestClassWithNoGivenSteps
    {
        [Behaviour]
        public void No_given_steps(Behaviour b)
        {
            b.Then(
                () => Assert.That(1, Is.EqualTo(2))
            );
        }
    }

    [BehaviourFixture]
    public class TestClassWithNoThenSteps
    {
        [Behaviour]
        public void No_then_steps(Behaviour b)
        {
            b.Given(
                () => Assert.That(1, Is.EqualTo(2))
            );
        }
    }
}
