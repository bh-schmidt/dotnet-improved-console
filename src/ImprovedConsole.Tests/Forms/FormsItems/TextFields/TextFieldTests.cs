using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextFields
{
    public class TextFieldTests
    {
        [Test]
        public void Should_validate_the_title()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new TextField(new FormEvents(), null!, new TextFieldOptions());
            }, "'title' cannot be null or empty.");
        }

        [Test]
        public void Should_validate_the_options()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new TextField(new FormEvents(), "Where did you come from?", null!);
            }, "'options' cannot be null.");
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                var field = new TextField(new FormEvents(), "Where did you come from?", new TextFieldOptions())
                    .OnConfirm(null!);
            });
        }

        [Test]
        public void Should_read_optional_text()
        {
            using var mocker = new ConsoleMock();

            mocker
                .Setup()
                .ReadLineReturns(new[]
                {
                    ""
                });

            var options = new TextFieldOptions()
            {
                Required = false
            };

            bool onConfirmCalled = false;
            string? result = null;

            FormEvents events = new FormEvents();

            var field = new TextField(events, "Where did you come from?", options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            var answer = field.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"Where did you come from?

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
                    "NY"
                });

            var options = new TextFieldOptions()
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

            var field = new TextField(events, "Where did you come from?", options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            var answer = field.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"Where did you come from?
NY
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(field);

            onConfirmCalled.Should().BeTrue();
            result.Should().Be("NY");
        }
    }
}
