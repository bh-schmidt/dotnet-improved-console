using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.MultiSelects;

namespace ImprovedConsole.Tests.Forms.FormsItems.MultiSelects
{
    public class MultiSelectTests
    {
        [Test]
        public void Should_select_the_first_two()
        {
            LinkedList<PossibilityItem> onChangeList = new();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using ConsoleMock mock = new();
            mock.Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                ]);

            FormEvents events = new();

            string[] colors = ["Red", "Green", "Blue",];
            MultiSelect select = new(events, "Select the colors", colors, new MultiSelectOptions());

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            string output = mock.GetOutput();

            output.Should().Be(
@"Select the colors
   [x] Red
 > [x] Green
   [ ] Blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            onChangeList.Should().HaveCount(2);
            onChangeList.Should().Contain(i => i.Value == "Red");
            onChangeList.Should().Contain(i => i.Value == "Green");

            onConfirmList.Should().HaveCount(2);
            onConfirmList.Should().Contain(i => i.Value == "Red");
            onConfirmList.Should().Contain(i => i.Value == "Green");
        }

        [Test]
        public void Should_select_all_and_deselect()
        {
            LinkedList<PossibilityItem> onChangeList = new();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using ConsoleMock mock = new();
            mock.Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                ]);

            FormEvents events = new();

            string[] colors = ["Red", "Green", "Blue",];
            MultiSelect select = new(events, "Select the colors", colors, new() { Required = false });
            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            string output = mock.GetOutput();

            output.Should().Be(
@"Select the colors
   [ ] Red
   [ ] Green
 > [ ] Blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            onChangeList.Should().HaveCount(6);
            onChangeList.Where(i => i.Value == "Red").Should().HaveCount(2);
            onChangeList.Where(i => i.Value == "Green").Should().HaveCount(2);
            onChangeList.Where(i => i.Value == "Blue").Should().HaveCount(2);

            onConfirmList.Should().HaveCount(0);
        }

        [Test]
        public void Should_show_error_when_there_is_no_selection()
        {
            LinkedList<PossibilityItem> onChangeList = new();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using ConsoleMock mock = new();
            mock.Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Enter,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                ]);

            FormEvents events = new();

            string[] colors = ["Red", "Green", "Blue",];
            MultiSelect select = new(events, "Select the colors", colors, new MultiSelectOptions { Required = true });
            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            string output = mock.GetOutput();

            output.Should().Be(
@"Select the colors
 > [x] Red
   [ ] Green
   [ ] Blue
Select at least one option
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            onChangeList.Should().HaveCount(1);
            onChangeList.Should().Contain(i => i.Value == "Red");

            onConfirmList.Should().HaveCount(1);
            onConfirmList.Should().Contain(i => i.Value == "Red");
        }

        [Test]
        public void Should_go_up_and_deselect_last()
        {
            LinkedList<PossibilityItem> onChangeList = new();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using ConsoleMock mock = new();
            mock.Setup()
                .ReadKeyReturns(
                [
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                ]);


            PossibilityItem[] items =
            [
                new PossibilityItem("Red") { Checked = true },
                new PossibilityItem("Green") { Checked = true },
                new PossibilityItem("Blue") { Checked = true },
            ];

            FormEvents events = new();

            MultiSelect select = new(events, "Select the colors", () => items, new MultiSelectOptions { Required = true });
            ImprovedConsole.Forms.Fields.IFieldAnswer answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            string output = mock.GetOutput();

            output.Should().Be(
@"Select the colors
   [x] Red
   [x] Green
 > [ ] Blue
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(select);

            onChangeList.Should().HaveCount(1);
            onChangeList.Should().Contain(i => i.Value == "Blue");

            onConfirmList.Should().HaveCount(2);
            onConfirmList.Should().Contain(i => i.Value == "Red");
            onConfirmList.Should().Contain(i => i.Value == "Green");
        }
    }
}
