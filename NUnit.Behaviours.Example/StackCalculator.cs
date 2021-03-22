using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace NUnit.Behaviours.Example
{
    [BehaviourFixture]
    public class StackCalculatorTests
    {
        private StackCalculator<int> calculator;
        private int value;

        [Behaviour]
        public void Entering_a_number(Behaviour b)
        {
            b.Given(
                () => Given_a_calculator(),
                () => Given_I_enter(12)
            );

            b.When(
                () => Reading_the_value()
            );

            b.Then(
                () => Then_result_should_be(12)
            );
        }

        [Behaviour]
        public void Entering_2_numbers(Behaviour b)
        {
            b.Given(
                () => Given_a_calculator(),
                () => Given_I_enter(12),
                () => Given_I_enter(23)
            );

            b.When(
                () => Reading_the_value()
            );

            b.Then(
                () => Then_result_should_be(23)
            );
        }

        [Behaviour]
        public void Adding_2_numbers_together(Behaviour b)
        {
            b.Given(
                () => Given_a_calculator(),
                () => Given_I_enter(12),
                () => Given_I_enter(23),
                () => Given_I_apply_operator(new Plus())
            );

            b.When(
                () => Reading_the_value()
            );

            b.Then(
                () => Then_result_should_be(35)
            );
        }

        [Behaviour]
        public void Subtracting_2_numbers(Behaviour b)
        {
            b.Given(
                () => Given_a_calculator(),
                () => Given_I_enter(42),
                () => Given_I_enter(29),
                () => Given_I_apply_operator(new Subtract())
            );

            b.When(
                () => Reading_the_value()
            );

            b.Then(
                () => Then_result_should_be(13)
            );
        }

        [Behaviour]
        public void Multiplying_2_numbers_together(Behaviour b)
        {
            b.Given(
                () => Given_a_calculator(),
                () => Given_I_enter(4),
                () => Given_I_enter(7),
                () => Given_I_apply_operator(new Multiply())
            );

            b.When(
                () => Reading_the_value()
            );

            b.Then(
                () => Then_result_should_be(28)
            );
        }

        [Behaviour]
        public void Putting_it_all_together(Behaviour b)
        {
            // ((2 (4 7 *) +) 6 -)
            b.Given(
                () => Given_a_calculator(),
                () => Given_I_enter(2),
                () => Given_I_enter(4),
                () => Given_I_enter(7),
                () => Given_I_apply_operator(new Multiply()),
                () => Given_I_apply_operator(new Plus()),
                () => Given_I_enter(6),
                () => Given_I_apply_operator(new Subtract())
            );

            b.When(
                () => Reading_the_value()
            );

            // (2 + (4 * 7)) - 6
            b.Then(
                () => Then_result_should_be(24)
            );
        }

        private void Given_a_calculator()
        {
            calculator = new StackCalculator<int>();
        }

        private void Given_I_enter(int value)
        {
            calculator.Push(value);
        }

        private void Given_I_apply_operator(IOperation<int> operation)
        {
            calculator.Apply(operation);
        }

        private void Reading_the_value()
        {
            value = calculator.GetValue();
        }

        private void Then_result_should_be(int expected)
        {
            Assert.That(value, Is.EqualTo(expected));
        }
    }

    public interface IOperation<T>
    {
        ImmutableStack<T> Apply(ImmutableStack<T> stack);
    }

    public abstract class BinaryOperation<T> : IOperation<T>
    {
        public ImmutableStack<T> Apply(ImmutableStack<T> stack)
        {
            var (b, stack2) = (stack.Peek(), stack.Pop());
            var (a, stack3) = (stack2.Peek(), stack2.Pop());

            return stack3.Push(Apply(a, b));
        }

        protected abstract T Apply(T a, T b);
    }

    public class Plus : BinaryOperation<int>
    {
        protected override int Apply(int a, int b)
        {
            return a + b;
        }
    }

    public class Subtract : BinaryOperation<int>
    {
        protected override int Apply(int a, int b)
        {
            return a - b;
        }
    }

    public class Multiply : BinaryOperation<int>
    {
        protected override int Apply(int a, int b)
        {
            return a * b;
        }
    }

    public class StackCalculator<T>
    {
        private ImmutableStack<T> stack = ImmutableStack<T>.Empty;

        public void Push(T item)
        {
            stack = stack.Push(item);
        }

        public void Apply(IOperation<T> op)
        {
            stack = op.Apply(stack);
        }

        public T GetValue()
        {
            return stack.Peek();
        }
    }
}