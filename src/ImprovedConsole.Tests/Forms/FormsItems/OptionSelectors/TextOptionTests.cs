using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Tests.Forms.FormsItems.OptionSelectors
{
    public class TextOptionTests
    {
        [Test]
        public void Should_validate_title()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new TextOption(new FormEvents(), null!, Array.Empty<string>(), new TextOptionOptions());
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
                string[] colors = ["blue", "red"];
                new TextOption(new FormEvents(), "Witch color do you want?", colors, null!);
            });
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                string[] colors = ["blue", "red"];
                new TextOption(new FormEvents(), "Witch color do you want?", colors, null!)
                    .OnConfirm(null!);
            });
        }

        [Test]
        public void Should_read_optional_option()
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    ""
                ]);

            TextOptionOptions options = new()
            {
                Required = false
            };

            bool onConfirmCalled = false;
            string? result = null;

            FormEvents events = new();

            string[] colors = ["blue", "red"];
            TextOption field = new TextOption(events, "Witch color do you want?", colors, options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = field.Run();

            string output = mocker.GetOutput();

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
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    "",
                    "green",
                    "red"
                ]);

            TextOptionOptions options = new()
            {
                Required = true
            };

            bool onConfirmCalled = false;
            string? result = null;

            FormEvents events = new();
            events.ReprintRequested += () =>
            {
                ConsoleWriter.Clear();
            };

            string[] colors = ["blue", "red"];
            TextOption field = new TextOption(events, "Witch color do you want?", colors, options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = field.Run();

            string output = mocker.GetOutput();

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
