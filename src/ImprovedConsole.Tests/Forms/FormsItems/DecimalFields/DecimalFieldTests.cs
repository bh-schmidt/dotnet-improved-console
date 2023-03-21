using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.DecimalFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.DecimalFields
{
    public class DecimalFieldTests
    {
        [Test]
        public void Should_validate_the_title()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new DecimalField(new FormEvents(), null!, new DecimalFieldOptions());
            });
        }

        [Test]
        public void Should_validate_the_options()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new DecimalField(new FormEvents(), "How old are you?", null!);
            });
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                var field = new DecimalField(new FormEvents(), "How old are you?", new DecimalFieldOptions())
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
                    "test",
                    ""
                });

            var options = new DecimalFieldOptions()
            {
                Required = false
            };

            bool onConfirmCalled = false;
            decimal? result = null;

            FormEvents events = new FormEvents();
            events.ReprintRequested += () => ConsoleWriter.Clear();

            var field = new DecimalField(events, "How old are you?", options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            var answer = field.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"How old are you?

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
                    "test",
                    "25.5"
                });

            var options = new DecimalFieldOptions()
            {
                Required = true
            };

            bool onConfirmCalled = false;
            decimal? result = null;

            FormEvents events = new FormEvents();
            events.ReprintRequested += () => ConsoleWriter.Clear();

            events.ReprintRequested += () =>
            {
                ConsoleWriter.Clear();
            };

            var field = new DecimalField(events, "How old are you?", options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            var answer = field.Run();

            var output = mocker.GetOutput();

            output.Should().Be(
@"How old are you?
25.5
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(field);

            onConfirmCalled.Should().BeTrue();
            result.Should().Be(25.5M);
        }
    }
}
