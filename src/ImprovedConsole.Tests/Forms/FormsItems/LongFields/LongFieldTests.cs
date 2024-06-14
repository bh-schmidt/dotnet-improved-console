using ImprovedConsole.ConsoleMockers;
using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.LongFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.LongFields
{
    public class LongFieldTests
    {
        [Test]
        public void Should_validate_the_title()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new LongField(new FormEvents(), null!, new LongFieldOptions());
            });
        }

        [Test]
        public void Should_validate_the_options()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                new LongField(new FormEvents(), "How old are you?", null!);
            });
        }

        [Test]
        public void Should_validate_the_on_confirm_event()
        {
            Assert.Catch(() =>
            {
                LongField field = new LongField(new FormEvents(), "How old are you?", new LongFieldOptions())
                    .OnConfirm(null!);
            });
        }

        [Test]
        public void Should_read_optional_text()
        {
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    "test",
                    ""
                ]);

            LongFieldOptions options = new()
            {
                Required = false
            };

            bool onConfirmCalled = false;
            long? result = null;

            FormEvents events = new();
            events.ReprintRequested += () => ConsoleWriter.Clear();

            LongField field = new LongField(events, "How old are you?", options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = field.Run();

            string output = mocker.GetOutput();

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
            using ConsoleMock mocker = new();

            mocker
                .Setup()
                .ReadLineReturns(
                [
                    "",
                    "test",
                    "25"
                ]);

            LongFieldOptions options = new()
            {
                Required = true
            };

            bool onConfirmCalled = false;
            long? result = null;

            FormEvents events = new();
            events.ReprintRequested += () => ConsoleWriter.Clear();

            events.ReprintRequested += () =>
            {
                ConsoleWriter.Clear();
            };

            LongField field = new LongField(events, "How old are you?", options)
                .OnConfirm(value =>
                {
                    onConfirmCalled = true;
                    result = value;
                });

            ImprovedConsole.Forms.Fields.IFieldAnswer answer = field.Run();

            string output = mocker.GetOutput();

            output.Should().Be(
@"How old are you?
25
");

            answer.Should().NotBeNull();
            answer.Field.Should().Be(field);

            onConfirmCalled.Should().BeTrue();
            result.Should().Be(25);
        }
    }
}
