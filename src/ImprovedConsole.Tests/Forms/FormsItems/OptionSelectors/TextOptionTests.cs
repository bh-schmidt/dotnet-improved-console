using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextOptions
{
    public class TextOptionTests
    {
        [Test]
        public void Should_validate_title()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new TextOption(new FormEvents(), null!, new string[] { }, new TextOptionOptions());
            });
        }

        [Test]
        public void Should_validate_available_options()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new TextOption(new FormEvents(), "Witch color do you want?", (Func<IEnumerable<string>>)null!, new TextOptionOptions());
            });
        }

        [Test]
        public void Should_validate_options()
        {
            Assert.Catch<ArgumentNullException>(() =>
            {
                new TextOption(new FormEvents(), "Witch color do you want?", new string[] { "blue", "red" }, null!);
            });
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                new TextOption(new FormEvents(), "Witch color do you want?", new string[] { "blue", "red" }, null!)
                    .OnConfirm(null!);
            });
        }

        [Test]
        public void Should_read_optional_option()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadLineReturns(new[]
                {
                    ""
                });

            var options = new TextOptionOptions()
            {
                Required = false
            };

            bool onConfirmCalled = false;
            string? result = null;

            FormEvents events = new FormEvents();

            var field = new TextOption(events, "Witch color do you want?", new string[] { "blue", "red" }, options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            var answer = field.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"Witch color do you want? (blue/red)

");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(field);

            onConfirmCalled.Should().BeTrue();
            result.Should().BeNull();
        }

        [Test]
        public void Should_read_required_text()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadLineReturns(new[]
                {
                    "",
                    "green",
                    "red"
                });

            var options = new TextOptionOptions()
            {
                Required = true
            };

            bool onConfirmCalled = false;
            string? result = null;

            FormEvents events = new FormEvents();
            events.ReprintRequested += () =>
            {
                ConsoleWriter.Clear();
            };

            var field = new TextOption(events, "Witch color do you want?", new string[] { "blue", "red" }, options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            var answer = field.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"Witch color do you want? (blue/red)
red
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(field);

            onConfirmCalled.Should().BeTrue();
            result.Should().Be("red");
        }
    }
}
