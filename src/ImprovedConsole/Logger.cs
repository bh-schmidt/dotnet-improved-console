namespace ImprovedConsole
{
    public class Logger
    {
        public static void Write(object value, LoggerOptions? options = null)
        {
            ConsoleColor currentColor = ConsoleWriter.GetForegroundColor();
            ConsoleColor currentBackgroundColor = ConsoleWriter.GetBackgroundColor();

            if (options?.Color is not null)
                ConsoleWriter.SetForegroundColor(options.Color.Value);

            if (options?.BackgroundColor is not null)
                ConsoleWriter.SetBackgroundColor(options.BackgroundColor.Value);

            ConsoleWriter.Write(value);

            ConsoleWriter.SetForegroundColor(currentColor);
            ConsoleWriter.SetBackgroundColor(currentBackgroundColor);
        }

        public static void WriteLine(object value, LoggerOptions? options = null)
        {
            Write(value, options);
            ConsoleWriter.WriteLine();
        }
    }

    public class LoggerOptions
    {
        public ConsoleColor? Color { get; set; }
        public ConsoleColor? BackgroundColor { get; set; }
    }
}
