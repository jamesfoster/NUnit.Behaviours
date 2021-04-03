namespace NUnit.Behaviours.TestFixtures
{
    [BehaviourFixture]
    public class TestClassWithTooManyArgs
    {
        [Behaviour]
        public void Too_many_args(Behaviour b, int i, string s) { }
    }
}
