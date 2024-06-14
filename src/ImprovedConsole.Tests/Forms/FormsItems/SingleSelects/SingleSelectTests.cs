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
                string[] colors = ["red", "green", "blue"];
                new SingleSelect(null!, colors, new SingleSelectOptions { });
            });
        }

        [Test]
        public void Should_throw_possibilities_is_null_or_empty()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new SingleSelect("What color do you pick?", (Func<IEnumerable<PossibilityItem>>)null!, new SingleSelectOptions { });
            });
        }

        [Test]
        public void Should_select_the_green_after_blue()
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                ]);

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            string[] colors = ["red", "green", "blue"];
            SingleSelect select = new SingleSelect(
                "What color do you pick?",
                colors,
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select.Run();

            string output = mocker.GetOutput();

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
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.Spacebar,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                ]);

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            FormEvents events = new();

            string[] colors = ["red", "green", "blue"];
            SingleSelect select = new SingleSelect(
                "What color do you pick?",
                colors,
                new SingleSelectOptions
                {
                    Required = false
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select.Run();

            string output = mocker.GetOutput();

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
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.Enter,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                ]);

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            string[] colors = ["red", "green", "blue"];
            SingleSelect select = new SingleSelect(
                "What color do you pick?",
                colors,
                new SingleSelectOptions
                {
                    Required = true,
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select.Run();

            string output = mocker.GetOutput();

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
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                ]);

            LinkedList<PossibilityItem> selectedValues = new();
            PossibilityItem? confirmedValue = null;

            string[] colors = ["red", "green", "blue"];
            SingleSelect select = new SingleSelect(
                "What color do you pick?",
                colors,
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select.Run();

            string output = mocker.GetOutput();

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
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.UpArrow,
                    ConsoleKey.UpArrow,
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter
                ]);

            LinkedList<PossibilityItem>? selectedValues = new();
            PossibilityItem? confirmedValue = null;

            string[] colors = ["red", "green", "blue"];
            SingleSelect select = new SingleSelect(
                "What color do you pick?",
                colors,
                new SingleSelectOptions
                {
                })
                .OnChange(value => selectedValues.AddLast(value))
                .OnConfirm(value => confirmedValue = value);

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select.Run();

            string output = mocker.GetOutput();

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
        public void Should_select_the_same_values_inputted()
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.Enter
                ]);

            PossibilityItem? confirmedValue = null;

            SingleSelect select = new(
                "What color do you pick?",
                () => new[] {
                    new PossibilityItem("red") { Checked = true },
                    new PossibilityItem("green") { Checked = false },
                    new PossibilityItem("blue") { Checked = false },
                },
                new SingleSelectOptions { });

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select
                .OnConfirm(value => confirmedValue = value)
                .Run();

            string output = mocker.GetOutput();

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
