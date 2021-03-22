using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Behaviours
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BehaviourFixtureAttribute : NUnitAttribute, IFixtureBuilder
    {
        public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
        {
            var methods = typeInfo.GetMethodsWithAttribute<BehaviourAttribute>(inherit: false);

            var fixtureSuite = new TestSuite(typeInfo);
            fixtureSuite.ApplyAttributesToTest(typeInfo.Type.GetTypeInfo());

            foreach (var method in methods)
            {
                var builder = new NUnitTestFixtureBuilder();

                var fixture = builder.BuildFrom(typeInfo, new MethodFilter(method));

                fixture.FullName = $"{typeInfo.FullName}.{method.Name}";

                fixtureSuite.Add(fixture);
            }

            yield return fixtureSuite;
        }

        private class MethodFilter : IPreFilter
        {
            private IMethodInfo method;

            public MethodFilter(IMethodInfo method)
            {
                this.method = method;
            }

            public bool IsMatch(Type type) => true;

            public bool IsMatch(Type type, MethodInfo method) => method.Name == this.method.Name;
        }
    }
}