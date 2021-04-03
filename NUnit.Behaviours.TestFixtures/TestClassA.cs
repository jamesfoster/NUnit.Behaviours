using NUnit.Framework;

using System;

namespace NUnit.Behaviours.TestFixtures
{
    [BehaviourFixture]
    public class TestClassA
    {
        [Behaviour]
        public void Behaviour1(Behaviour b)
        {
            b.Given(() => Given_nothing());
            b.Then(() => Assert.That(1, Is.EqualTo(1)));
        }

        [Behaviour]
        public void Behaviour2(Behaviour b)
        {
            b.Given(() => Given_nothing());
            b.Then(() => Assert.That(1, Is.EqualTo(2)));
        }

        private void Given_nothing() { }
    }
}
