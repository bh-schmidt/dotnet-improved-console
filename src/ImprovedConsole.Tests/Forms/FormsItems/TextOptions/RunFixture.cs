﻿using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Tests.Forms.FormsItems.OptionSelectors
{
    [TestFixture]
    public class RunFixture
    {
        private static IEnumerable<IFieldWrapper> Fields() => FieldMap.Fields();
        private static IEnumerable<IFieldWrapper> StructFields() => FieldMap
            .Fields()
            .Where(e => e.Type.IsValueType);
        private static IEnumerable<IFieldWrapper> StringField() => FieldMap
            .Fields()
            .Where(e => e.Type == typeof(string));

        [TestCaseSource(nameof(Fields))]
        public void Should_select_the_option<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    wrapper.First!.ToString()!
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Options(wrapper.Options)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.First);

            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();
            var optionsString = string.Join("/", wrapper.Options.Select(e => e!.ToString()));
            output.Should().Be(
$"""
Select the options. ({optionsString})
{wrapper.First}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_select_the_option_after_required_selection_error<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    "",
                    wrapper.First!.ToString()!
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Options(wrapper.Options)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.First);

            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();
            var optionsString = string.Join("/", wrapper.Options.Select(e => e!.ToString()));
            output.Should().Be(
$"""
Select the options. ({optionsString})
 * This field is required.
{wrapper.First}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_skip_the_field_because_it_is_not_required<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    ""
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Required(false)
                .Options(wrapper.Options)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.Default);

            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();
            var optionsString = string.Join("/", wrapper.Options.Select(e => e!.ToString()));
            output.Should().Be(
$"""
Select the options. ({optionsString})


""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_only_return_the_answer_because_there_is_the_initial_value<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    ""
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Set(wrapper.First)
                .Options(wrapper.Options)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.First);

            onConfirmCalled.Should().BeFalse();

            string output = mocker.GetOutput();
            output.Should().BeEmpty();
        }

        [TestCaseSource(nameof(StructFields))]
        public void Should_show_the_conversion_error_and_then_skip_the_field<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    "conversion error",
                    ""
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Required(false)
                .Options(wrapper.Options)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.Default);

            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();
            var optionsString = string.Join("/", wrapper.Options.Select(e => e!.ToString()));
            output.Should().Be(
$"""
Select the options. ({optionsString})
 * Could not convert the value.


""");
        }

        [TestCaseSource(nameof(StringField))]
        public void Should_show_the_invalid_selection_error_and_then_skip_the_field<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    "wrong selection",
                    ""
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Required(false)
                .Options(wrapper.Options)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.Default);

            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();
            var optionsString = string.Join("/", wrapper.Options.Select(e => e!.ToString()));
            output.Should().Be(
$"""
Select the options. ({optionsString})
 * Invalid option.


""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_return_the_informed_default_because_there_was_no_selection<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    string.Empty
                ]);

            bool onConfirmCalled = false;

            FormEvents events = new();

            wrapper.Field
                .Title("Select the options.")
                .Options(wrapper.Options)
                .Default(wrapper.First)
                .OnConfirm(value => onConfirmCalled = true)
                .ValidateField();

            var answer = (TextOptionAnswer<T>)wrapper.Field.Run();
            answer.Answer.Should().Be(wrapper.First);

            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();
            var optionsString = string.Join("/", wrapper.Options.Select(e => e!.ToString()));
            output.Should().Be(
$"""
Select the options. ({optionsString})


""");
        }
    }
}
