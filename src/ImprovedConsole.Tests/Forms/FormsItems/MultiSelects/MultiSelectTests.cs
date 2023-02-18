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
            var onChangeList = new LinkedList<PossibilityItem>();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using var mock = new ConsoleMock();
            mock.Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.Spacebar,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                });

            var events = new FormEvents();

            var select = new MultiSelect(events, "Select the colors", new[] { "Red", "Green", "Blue", }, new MultiSelectOptions());

            var answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            var output = mock.GetOutput();

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
            var onChangeList = new LinkedList<PossibilityItem>();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using var mock = new ConsoleMock();
            mock.Setup()
                .ReadKeyReturns(new[]
                {
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
                });

            var events = new FormEvents();

            var select = new MultiSelect(events, "Select the colors", new[] { "Red", "Green", "Blue", }, new MultiSelectOptions());
            var answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            var output = mock.GetOutput();

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
            var onChangeList = new LinkedList<PossibilityItem>();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using var mock = new ConsoleMock();
            mock.Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.DownArrow,
                    ConsoleKey.Enter,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                });

            var events = new FormEvents();

            var select = new MultiSelect(events, "Select the colors", new[] { "Red", "Green", "Blue", }, new MultiSelectOptions { Required = true });
            var answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            var output = mock.GetOutput();

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
            var onChangeList = new LinkedList<PossibilityItem>();
            IEnumerable<PossibilityItem>? onConfirmList = null;

            using var mock = new ConsoleMock();
            mock.Setup()
                .ReadKeyReturns(new[]
                {
                    ConsoleKey.UpArrow,
                    ConsoleKey.Spacebar,
                    ConsoleKey.Enter,
                });


            var items = new[]
            {
                new PossibilityItem("Red") { Checked=true },
                new PossibilityItem("Green") { Checked=true },
                new PossibilityItem("Blue") { Checked=true },
            };

            var events = new FormEvents();

            var select = new MultiSelect(events, "Select the colors", () => items, new MultiSelectOptions { Required = true });
            var answer = select
                .OnChange(i => onChangeList.AddLast(i))
                .OnConfirm(items => onConfirmList = items)
                .Run();

            var output = mock.GetOutput();

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
