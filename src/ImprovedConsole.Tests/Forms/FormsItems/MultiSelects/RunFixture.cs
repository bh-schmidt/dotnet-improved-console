using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms.Fields.MultiSelects;
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
                .ReadKey(wrapper.Options
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
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_deselect_all_options_and_end_up_at_the_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(wrapper.Options
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
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_return_the_informed_default_values_because_no_item_was_selected<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(ConsoleKey.Enter);

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Required(false)
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
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_show_error_when_there_is_no_selection<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(
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

            var builder = new StringBuilder();

            builder.AppendLine($" > [x] {wrapper.First}");

            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");

            builder.Append($"   [ ] {wrapper.Last}");

            string output = mocker.GetOutput();
            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}
 * This field is required.
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_up_and_end_up_at_the_last_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(ConsoleKey.UpArrow, ConsoleKey.Enter);

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
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_go_down_and_end_up_at_the_first_position<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(wrapper.Options.Select(e => ConsoleKey.DownArrow).Append(ConsoleKey.Enter));

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
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }

        [TestCaseSource(nameof(Fields))]
        public void Should_set_the_external_value_but_edit_and_set_another_value<T>(FieldWrapper<T> wrapper)
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKey(ConsoleKey.DownArrow, ConsoleKey.Spacebar, ConsoleKey.Enter);

            int onChangeCount = 0;
            bool onConfirmCalled = false;

            wrapper.Field
               .Title("Select your options.")
               .Options(wrapper.Options)
               .Set([wrapper.Last])
               .OnChange(value => onChangeCount++)
               .OnConfirm(value => onConfirmCalled = true);

            var answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().Contain(wrapper.Last);

            onChangeCount.Should().Be(0);
            onConfirmCalled.Should().BeTrue();
            onConfirmCalled = false;

            string output = mocker.GetOutput();
            output.Should().BeEmpty();

            wrapper.Field.SetEdition();
            answer = (MultiSelectAnswer<T>)wrapper.Field.Run();
            answer.Selections!.Should().Contain(wrapper.First);

            onChangeCount.Should().Be(1);
            onConfirmCalled.Should().BeTrue();

            var builder = new StringBuilder();
            builder.AppendLine($" > [x] {wrapper.First}");
            foreach (var option in wrapper.Options.Skip(1).SkipLast(1))
                builder.AppendLine($"   [ ] {option}");
            builder.Append($"   [x] {wrapper.Last}");

            output = mocker.GetOutput();
            output.Should().Be(
$"""
Select your options.
{string.Concat(builder)}
up: ↑ k   down: ↓ j   toggle: SPACE   confirm: ENTER

""");
        }
    }
}
