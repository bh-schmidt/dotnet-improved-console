using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Tests.Forms.FormsItems.SingleSelects
{
    public class SingleSelectTests
    {
        [Test]
        public void Should_throw_title_is_null_or_empty()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new SingleSelect(new FormEvents(), null!, new[] { "red", "green", "blue" }, new SingleSelectOptions { });
            });
        }

        [Test]
        public void Should_throw_possibilities_is_null_or_empty()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new SingleSelect(new FormEvents(), "What color do you pick?", (Func<IEnumerable<PossibilityItem>>)null!, new SingleSelectOptions { });
            });
        }

        [Test]
        public void Should_select_the_green_after_blue()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                });

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            var events = new FormEvents();

            var select = new SingleSelect(
                events,
                "What color do you pick?",
                new[] { "red", "green", "blue" },
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            var answer = select.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"What color do you pick?
   [ ] red
 > [*] green
   [ ] blue
");
            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            selectedValues.Should().HaveCount(2);
            selectedValues.Should().Contain(e => e.Value == "blue");
            selectedValues.Should().Contain(e => e.Value == "green");

            confirmedValue.Should().NotBeNull();
            confirmedValue!.Value.Should().Be("green");
        }

        [Test]
        public void Should_unselect_the_selection()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.Spacebar,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                });

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            var events = new FormEvents();

            var select = new SingleSelect(
                events,
                "What color do you pick?",
                new[] { "red", "green", "blue" },
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            var answer = select.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"What color do you pick?
 > [ ] red
   [ ] green
   [ ] blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            selectedValues.Should().HaveCount(2);
            selectedValues.Should().Contain(e => e.Value == "red");

            confirmedValue.Should().BeNull();
        }

        [Test]
        public void Should_show_error_before_select()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.Enter,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                });

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            var events = new FormEvents();

            var select = new SingleSelect(
                events,
                "What color do you pick?",
                new[] { "red", "green", "blue" },
                new SingleSelectOptions
                {
                    Required = true,
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            var answer = select.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"What color do you pick?
 > [*] red
   [ ] green
   [ ] blue
Select one option
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            selectedValues.Should().HaveCount(1);
            selectedValues.Should().Contain(e => e.Value == "red");

            confirmedValue.Should().NotBeNull();
            confirmedValue!.Value.Should().Be("red");
        }

        [Test]
        public void Should_select_red_after_go_down()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                });

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            var events = new FormEvents();

            var select = new SingleSelect(
                events,
                "What color do you pick?",
                new[] { "red", "green", "blue" },
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            var answer = select.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"What color do you pick?
 > [*] red
   [ ] green
   [ ] blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            selectedValues.Should().HaveCount(1);
            selectedValues.Should().Contain(e => e.Value == "red");

            confirmedValue.Should().NotBeNull();
            confirmedValue!.Value.Should().Be("red");
        }

        [Test]
        public void Should_select_red_after_go_up()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.UpArrow,
                    ConsoleKey.UpArrow,
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                });

            LinkedList<PossibilityItem>? selectedValues = new();
            PossibilityItem? confirmedValue = null;

            var events = new FormEvents();

            var select = new SingleSelect(
                events,
                "What color do you pick?",
                new[] { "red", "green", "blue" },
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            var answer = select.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"What color do you pick?
 > [*] red
   [ ] green
   [ ] blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            selectedValues.Should().HaveCount(1);
            selectedValues.Should().Contain(e => e.Value == "red");

            confirmedValue.Should().NotBeNull();
            confirmedValue!.Value.Should().Be("red");
        }

        [Test]
        public void Should_select_the_same_values_inputed()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.Enter
                });

            PossibilityItem? confirmedValue = null;

            var events = new FormEvents();

            var select = new SingleSelect(
                events,
                "What color do you pick?",
                () => new[] {
                    new PossibilityItem("red") { Checked = true },
                    new PossibilityItem("green") { Checked = false },
                    new PossibilityItem("blue") { Checked = false },
                },
                new SingleSelectOptions { });

            var answer = select
                .OnConfirm(value => confirmedValue = value)
                .Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"What color do you pick?
 > [*] red
   [ ] green
   [ ] blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            confirmedValue.Should().NotBeNull();
            confirmedValue!.Value.Should().Be("red");
        }
    }
}
