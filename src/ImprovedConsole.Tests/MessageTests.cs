using ImprovedConsole.ConsoleMockers;

namespace ImprovedConsole.Tests
{
    public class MessageTests
    {
        [Test]
        public void Should_decipher_the_message()
        {
            var messages = Message.DecipherMessages("{color:1}this {background:100}message {color:red}should {background:blue}be {color:green}{background:black}deciphed{background:200}").ToArray();
            messages.Should().HaveCount(5);

            messages[0].Description.Should().Be("this ");
            messages[0].Color.Should().Be((ConsoleColor)1);
            messages[0].BackgroundColor.Should().BeNull();

            messages[1].Description.Should().Be("message ");
            messages[1].Color.Should().Be((ConsoleColor)1);
            messages[1].BackgroundColor.Should().Be((ConsoleColor)100);

            messages[2].Description.Should().Be("should ");
            messages[2].Color.Should().Be(ConsoleColor.Red);
            messages[2].BackgroundColor.Should().Be((ConsoleColor)100);

            messages[3].Description.Should().Be("be ");
            messages[3].Color.Should().Be(ConsoleColor.Red);
            messages[3].BackgroundColor.Should().Be(ConsoleColor.Blue);

            messages[4].Description.Should().Be("deciphed");
            messages[4].Color.Should().Be(ConsoleColor.Green);
            messages[4].BackgroundColor.Should().Be(ConsoleColor.Black);
        }

        [Test]
        public void Should_write_the_message_to_console()
        {
            using var mocker = new ConsoleMock();
            Message.Write("This should be printed");
            mocker.GetOutput().Should().Be(@"This should be printed");
        }
    }
}
