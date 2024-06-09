using System.Text;
using System.Text.RegularExpressions;

namespace ImprovedConsole
{
    public class Message
    {
        public Message(string description, ConsoleColor? color, ConsoleColor? backgroundColor)
        {
            Description = description;
            Color = color;
            BackgroundColor = backgroundColor;
        }

        public string Description { get; }
        public ConsoleColor? Color { get; }
        public ConsoleColor? BackgroundColor { get; }

        public static void WriteLine(string text)
        {
            Write(text);
            ConsoleWriter.WriteLine();
        }

        public static void Write(string text)
        {
            if (text is null)
                return;

            var messages = DecipherMessages(text);

            foreach (var message in messages)
            {
                Logger.Write(
                    message.Description,
                    new LoggerOptions
                    {
                        Color = message.Color,
                        BackgroundColor = message.BackgroundColor,
                    });
            }
        }

        public static string? RemoveColors(string? text)
        {
            if (text is null)
                return null;

            var messages = DecipherMessages(text);

            var builder = new StringBuilder();
            foreach (var message in messages)
                builder.Append(message.Description);

            return builder.ToString();
        }

        public static IEnumerable<Message> DecipherMessages(string text)
        {
            var matches = Regex.Matches(text, @"[{](color|background)[:]([-]?[a-zA-Z\d]{1,11})[}]");

            var lastIndex = 0;
            ConsoleColor? lastColor = null;
            ConsoleColor? lastBackgroundColor = null;

            foreach (Match match in matches)
            {
                if (match.Index != 0)
                {
                    var interlanDescription = text[lastIndex..match.Index];
                    if (interlanDescription.Length > 0)
                        yield return new Message(interlanDescription, lastColor, lastBackgroundColor);
                }

                lastIndex = match.Index + match.Length;

                string colorText = match.Groups[2].Value;

                if (match.Groups[1].Value.ToLower() == "color")
                {
                    lastColor = colorText == "default" ?
                        ConsoleWriter.GetForegroundColor() :
                        GetColor(colorText);
                    continue;
                }

                lastBackgroundColor = colorText == "default" ?
                    ConsoleWriter.GetBackgroundColor() :
                    GetColor(colorText);
            }

            var description = text[lastIndex..];
            if (description.Length > 0)
                yield return new Message(description, lastColor, lastBackgroundColor);

        }

        private static ConsoleColor GetColor(string text)
        {
            if (int.TryParse(text, out int number))
                return (ConsoleColor)number;

            return Enum.Parse<ConsoleColor>(text, true);
        }
    }
}
