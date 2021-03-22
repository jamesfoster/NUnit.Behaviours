# NUnit.Behaviours
A BDD style framework for writing tests


## Installation

```
Install-Package NUnit.Behaviours
```

## Usage

```csharp
using NUnit.Behaviours;
using NUnit.Framework;

namespace TestProject
{
    [BehaviourFixture]
    public class Tests
    {
        [Behaviour]
        public void Addition(Behaviour b)
        {
            b.Given(
                () => Given_a_value_of(1),
                () => And_a_value_of(3)
            );

            b.When(
                () => When_adding_the_values_together()
            );

            b.Then(
                () => Then_the_result_is(4)
            );
        }

        private int value1;
        private int value2;
        private int result;

        private void Given_a_value_of(int i) => value1 = i;
        private void And_a_value_of(int i) => value2 = i;
        private void When_adding_the_values_together() => result = value1 + value2;
        private void Then_the_result_is(int i) => Assert.That(result, Is.EqualTo(i));
    }
}
```

## Example output

The `[Behaviour]` attribute generates one test per assertion. It also ensures that the `Given` and `When` steps are only executed once.

```csharp
...
        [Behaviour]
        public void Error_adding_widget(Behaviour b)
        {
            b.Given(
                () => Given_a_user("joe.bloggs"),
                () => Given_a_widget_repository(),
                () => Given_an_error_occurs_adding_a_widget()
            );

            b.When(
                () => When_the_user_adds_a_widget("joe.bloggs", "foo")
            );

            b.Then(
                () => Then_an_error_message_is_returned("Failed creating widget 'foo'"),
                () => Then_the_widget_factory_lights_turn_red(),
                () => Then_an_inspector_is_dispacted_to_user("joe.bloggs")
            );
        }
...
```

![image](https://user-images.githubusercontent.com/196800/112069106-209b3d80-8b63-11eb-90c4-7e85fa567286.png)
