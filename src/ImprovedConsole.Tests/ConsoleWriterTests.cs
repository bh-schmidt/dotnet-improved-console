using ImprovedConsole.ConsoleMockers;

namespace ImprovedConsole.Tests
{
    public class ConsoleWriterTests
    {
        [Test]
        public void Should_break_line_without_line_break()
        {
            using ConsoleMock mocker = new();

            ConsoleWriter.Write("teste");

            for (int i = 0; i < 130; i++)
                ConsoleWriter.Write(' ');

            ConsoleWriter.Write("teste");

            string output = mocker.GetOutput();

            output.Should().Be(
@"teste                                                                                                                   
               teste");
        }

        [Test]
        public void Should_clear_the_line()
        {
            using ConsoleMock mocker = new();

            ConsoleWriter.WriteLine("teste")
                .WriteLine("teste")
                .WriteLine("teste")
                .SetCursorPosition(0, 1)
                .ClearLine();

            string output = mocker.GetOutput();

            output.Should().Be(
@"teste

teste
");
        }
    }
}
