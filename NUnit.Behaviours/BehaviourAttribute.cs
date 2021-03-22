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
            List<TestCaseParameters> data = new();
            InitializationLock locker = new();
            var categories = Category?.Split(',') ?? new string[0];

            try
            {
                var source = GetBehaviour(method);

                var length = source.Length;
                for (int i = 0; i < length; i++)
                {
                    var parms = source.GetTestPamameters(i, locker);

                    foreach (string cat in categories)
                    {
                        parms.Properties.Add(PropertyNames.Category, cat);
                    }

                    data.Add(parms);
                }
            }
            catch (Exception ex)
            {
                data.Clear();
                data.Add(new TestCaseParameters(ex));
            }

            return data;
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
