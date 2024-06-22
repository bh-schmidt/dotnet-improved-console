using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms.Fields.SingleSelects;
using System.Text;

namespace ImprovedConsole.Tests.Forms.FormsItems.SingleSelects
{
    [TestFixture]
    public class RunFixture
    {
        private static IEnumerable<object> Fields() => FieldMap.Fields();

        [TestCaseSource(nameof(Fields))]
        public void Should_select_the_last_option<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(
                [
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                ]);

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.Last);

            onChangeCalled.Should().BeTrue();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            foreach (var option in wrapper.Options.SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($" > [*] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_down_and_end_at_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(wrapper.Options
                    .Select(e => ConsoleKey.DownArrow)
                    .Append(ConsoleKey.Spacebar)
                    .Append(ConsoleKey.Enter));

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.First);

            onChangeCalled.Should().BeTrue();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [*] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_up_and_end_at_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(wrapper.Options
                    .Select(e => ConsoleKey.UpArrow)
                    .Append(ConsoleKey.Spacebar)
                    .Append(ConsoleKey.Enter));

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.First);

            onChangeCalled.Should().BeTrue();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [*] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_select_each_value_but_only_maintain_the_last_selected<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(wrapper.Options
                    .SelectMany(e => new[] { ConsoleKey.DownArrow, ConsoleKey.Spacebar })
                    .Append(ConsoleKey.Enter));

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.First);

            onChangeCalled.Should().BeTrue();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [*] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_select_the_first_field_and_deselect_because_the_field_is_not_required<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(
                    ConsoleKey.Spacebar,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter);

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Required(false)
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection.Should().Be(wrapper.Default);

            onChangeCalled.Should().BeTrue();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [ ] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_to_the_last_position_and_do_nothing_because_the_field_is_not_required<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(
                    ConsoleKey.UpArrow,
                    ConsoleKey.Enter);

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Required(false)
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection.Should().Be(wrapper.Default);

            onChangeCalled.Should().BeFalse();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            foreach (var option in wrapper.Options.SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($" > [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_only_confirm_the_values_that_were_preselected<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(ConsoleKey.Enter);

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Selected(wrapper.Last)
               .Options(wrapper.Options)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.Last);

            onChangeCalled.Should().BeFalse();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            foreach (var option in wrapper.Options.SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($" > [*] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_return_the_informed_default_value_because_no_item_was_selected<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(
                [
                    ConsoleKey.Enter
                ]);

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Options(wrapper.Options)
               .Required(false)
               .Default(wrapper.First)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.First);

            onChangeCalled.Should().BeFalse();
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [ ] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }


        [TestCaseSource(nameof(Fields))]
        public void Should_select_the_external_value_and_edit_the_value<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(
                [
                    ConsoleKey.UpArrow,
                    ConsoleKey.Enter
                ]);

            bool onChangeCalled = false;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select a value.")
               .Options(wrapper.Options)
               .Set(wrapper.First)
               .OnChange(value => onChangeCalled = true)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.First);

            onChangeCalled.Should().BeFalse();
            onConfirmCalled.Should().BeTrue();
            onConfirmCalled = false;

            string output = mocker.GetOutput();
            output.Should().BeEmpty();

            wrapper.Field.SetEdition();
            answer = (SingleSelectAnswer<T>)wrapper.Field.Run();
            answer.Selection!.Should().Be(wrapper.Last);

            onChangeCalled.Should().BeTrue();
            onConfirmCalled.Should().BeTrue();

            var builder = new StringBuilder();
            builder.AppendLine($"   [ ] {wrapper.First}");
            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");
            builder.Append($" > [*] {wrapper.Last}");

            output = mocker.GetOutput();
            output.Should().Be(
$"""
Select a value.
{string.Concat(builder)}

""");
        }
    }
}
