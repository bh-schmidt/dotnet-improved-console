using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms.Fields.MultiSelects;
using ImprovedConsole.Forms.Fields.SingleSelects;
using System.Text;

namespace ImprovedConsole.Tests.Forms.FormsItems.MultiSelects
{
    [TestFixture]
    internal class RunFixture
    {
        private static IEnumerable<object> Fields() => FieldMap.Fields();

        [TestCaseSource(nameof(Fields))]
        public void Should_select_all_options_and_end_up_at_the_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(wrapper.Options
                    .SelectMany(e => new[] {
                        ConsoleKey.DownArrow,
                        ConsoleKey.Spacebar,
                    })
                    .Append(ConsoleKey.Enter)
                );

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Options(wrapper.Options)
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().Contain(wrapper.Options);

            onChangeCount.Should().Be(wrapper.Options.Length);
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [x] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [x] {option}");

            builder.Append($"   [x] {wrapper.Last}");

            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_deselect_all_options_and_end_up_at_the_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(wrapper.Options
                    .SelectMany(e => new[] {
                        ConsoleKey.DownArrow,
                        ConsoleKey.Spacebar,
                    })
                    .Append(ConsoleKey.Enter)
                );

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Options(wrapper.Options)
               .Selected(wrapper.Options)
               .Required(false)
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().BeEmpty();

            onChangeCount.Should().Be(wrapper.Options.Length);
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [ ] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_return_the_informed_default_values_because_no_item_was_selected<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(ConsoleKey.Enter);

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Default([wrapper.First, wrapper.Last])
               .Options(wrapper.Options)
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().Contain([wrapper.First, wrapper.Last]);

            onChangeCount.Should().Be(0);
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [ ] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_show_error_when_there_is_no_selection<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                    ConsoleKey.Enter,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter);

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Options(wrapper.Options)
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().Contain(wrapper.First);

            onChangeCount.Should().Be(1);
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [x] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}
 * This field is required.

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_up_and_end_up_at_the_last_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(ConsoleKey.UpArrow, ConsoleKey.Enter);

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Required(false)
               .Options(wrapper.Options)
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().BeEmpty();

            onChangeCount.Should().Be(0);
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($"   [ ] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($" > [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_down_and_end_up_at_the_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(wrapper.Options.Select(e => ConsoleKey.DownArrow).Append(ConsoleKey.Enter));

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Required(false)
               .Options(wrapper.Options)
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().BeEmpty();

            onChangeCount.Should().Be(0);
            onConfirmCalled.Should().BeTrue();

            string output = mocker.GetOutput();

            var builder = new StringBuilder();

            builder.AppendLine($" > [ ] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}

""");
        }
    }
}
