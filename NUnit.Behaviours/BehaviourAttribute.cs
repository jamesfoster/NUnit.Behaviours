using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

using System;
using System.Collections.Generic;
using System.Security;

namespace NUnit.Behaviours
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class BehaviourAttribute : NUnitAttribute, ITestBuilder, IImplyFixture
    {
        private readonly NUnitTestCaseBuilder builder = new();

        public string? Category { get; set; }

        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
        {
            int count = 0;

            foreach (var parms in GetTestCasesFor(method))
            {
                count++;
                yield return builder.BuildTestMethod(method, suite, parms);
            }
        }

        [SecuritySafeCritical]
        private IEnumerable<TestCaseParameters> GetTestCasesFor(IMethodInfo method)
        {
            InitializationLock locker = new();
            var categories = Category?.Split(',') ?? new string[0];

            try
            {
                if (MethodHasTooManyArgs(method))
                {
                    return new[] { ErrorState("Too many arguments") };
                }
                if (IsMethodMissingBehaviourArg(method))
                {
                    return new[] { ErrorState("No behaviour argument") };
                }

                var source = GetBehaviour(method);

                if (source.GivenNames.Count == 0)
                {
                    return new[] { ErrorState("No given steps") };
                }
                if (source.ThenNames.Count == 0)
                {
                    return new[] { ErrorState("No then steps") };
                }

                List<TestCaseParameters> data = new();
                for (int i = 0; i < source.Length; i++)
                {
                    var parms = source.GetTestPamameters(i, locker);

                    foreach (string cat in categories)
                    {
                        parms.Properties.Add(PropertyNames.Category, cat);
                    }

                    data.Add(parms);
                }

                return data;
            }
            catch (Exception ex)
            {
                return new[] { new TestCaseParameters(ex) };
            }
        }

        private bool MethodHasTooManyArgs(IMethodInfo method)
        {
            var parameters = method.GetParameters();

            return parameters.Length > 1;
        }

        private bool IsMethodMissingBehaviourArg(IMethodInfo method)
        {
            var parameters = method.GetParameters();

            if (parameters.Length == 0) return true;

            var parameterType = parameters[0].ParameterType;

            return parameterType != typeof(Behaviour);
        }

        private TestCaseParameters ErrorState(string message)
        {
            var parms = new TestCaseParameters();

            parms.RunState = RunState.NotRunnable;
            parms.Properties.Set(PropertyNames.SkipReason, message);

            return parms;
        }

        private BehaviourRecorder GetBehaviour(IMethodInfo method)
        {
            var recorder = new BehaviourRecorder();

            var instance = method.TypeInfo.Construct(new object[0]);

            method.Invoke(instance, new object[] { recorder });

            return recorder;
        }
    }
}
