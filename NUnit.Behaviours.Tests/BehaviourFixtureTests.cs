using NUnit.Behaviours.TestFixtures;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Behaviours.Tests
{
    [BehaviourFixture]
    public class BehaviourFixtureTests
    {
        [Behaviour]
        public void Creates_one_fixture_per_behaviour(Behaviour b)
        {
            b.Given(
                () => Given_a_type<TestClassA>()
            );

            b.When(
                () => When_loading_fixtures()
            );

            b.Then(
                () => Then_the_number_of_fixtures_is(2),
                () => Then_there_is_a_fixture_for(nameof(TestClassA.Behaviour1)),
                () => Then_there_is_a_fixture_for(nameof(TestClassA.Behaviour2))
            );
        }

        [Behaviour]
        public void Fails_if_behaviour_does_not_accept_a_behaviour_argument(Behaviour b)
        {
            b.Given(
                () => Given_a_type<TestClassWithoutBehaviourArg>()
            );

            b.When(
                () => When_loading_fixtures()
            );

            b.Then(
                () => Then_there_is_an_error_with_message(
                    nameof(TestClassWithoutBehaviourArg.Missing_behaviour_arg),
                    "No behaviour argument"
                )
            );
        }

        [Behaviour]
        public void Fails_if_behaviour_has_too_many_arguments(Behaviour b)
        {
            b.Given(
                () => Given_a_type<TestClassWithTooManyArgs>()
            );

            b.When(
                () => When_loading_fixtures()
            );

            b.Then(
                () => Then_there_is_an_error_with_message(
                    nameof(TestClassWithTooManyArgs.Too_many_args),
                    "Too many arguments"
                )
            );
        }

        [Behaviour]
        public void Fails_if_behaviour_has_no_given_steps(Behaviour b)
        {
            b.Given(
                () => Given_a_type<TestClassWithNoGivenSteps>()
            );

            b.When(
                () => When_loading_fixtures()
            );

            b.Then(
                () => Then_there_is_an_error_with_message(
                    nameof(TestClassWithNoGivenSteps.No_given_steps),
                    "No given steps"
                )
            );
        }

        [Behaviour]
        public void Fails_if_behaviour_has_no_then_steps(Behaviour b)
        {
            b.Given(
                () => Given_a_type<TestClassWithNoThenSteps>()
            );

            b.When(
                () => When_loading_fixtures()
            );

            b.Then(
                () => Then_there_is_an_error_with_message(
                    nameof(TestClassWithNoThenSteps.No_then_steps),
                    "No then steps"
                )
            );
        }

        #region Steps

        private Type type;
        private ITypeInfo typeInfo;
        private List<TestSuite> fixtures;

        private void Given_a_type<T>()
        {
            type = typeof(T);
            typeInfo = new TypeWrapper(type);
        }

        private void When_loading_fixtures()
        {
            var attribute = new BehaviourFixtureAttribute();

            fixtures = attribute.BuildFrom(typeInfo).ToList();
        }

        private void Then_the_number_of_fixtures_is(int count)
        {
            Assert.That(fixtures, Has.Count.EqualTo(count));
        }

        private void Then_there_is_a_fixture_for(string name)
        {
            var fullName = $"{type.FullName}.{name}";

            Assert.That(
                fixtures.Select(x => x.FullName),
                Has.Exactly(1).EqualTo(fullName)
            );
        }

        private void Then_the_number_of_tests_for_fixture_is(string name, int count)
        {
            var fixture = GetFixtureForMethod(name);

            Assert.That(fixture.Tests, Has.Count.EqualTo(count));
        }

        private void Then_there_is_an_error_with_message(string name, string message)
        {
            var fixture = GetFixtureForMethod(name);
            var test = fixture.Tests.Single().Tests.Single();

            Assert.That(test.RunState, Is.EqualTo(RunState.NotRunnable));

            var skipReasons = test.Properties[PropertyNames.SkipReason];

            Assert.That(skipReasons, Has.Count.EqualTo(1));
            Assert.That(skipReasons[0], Is.EqualTo(message));
        }

        #endregion

        #region Helpers

        private TestSuite GetFixtureForMethod(string name)
        {
            var fullName = $"{type.FullName}.{name}";
            return fixtures.Single(x => x.FullName == fullName);
        }

        #endregion

    }
}