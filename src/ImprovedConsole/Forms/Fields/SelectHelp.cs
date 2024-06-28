using System.Text;

namespace ImprovedConsole.Forms.Fields
{
    public class SelectHelp
    {
        public static void Print()
        {
            var helpText =
"""
Usage:

go up:           ↑ k
go down:         ↓ j
toggle option:   SPACE
confirm:         ENTER

Press any key to continue...
""";
            ConsoleWriter.Clear();
            ConsoleWriter.WriteLine(helpText);
            ConsoleWriter.ReadKey();
        }
    }
}
