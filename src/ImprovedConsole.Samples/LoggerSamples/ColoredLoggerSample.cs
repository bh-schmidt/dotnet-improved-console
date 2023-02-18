namespace ImprovedConsole.Samples.LoggerSamples
{
    public static class ColoredLoggerSample
    {
        public static void Run()
        {
            Logger.WriteLine(
                "This is a colored message.",
                new LoggerOptions
                {
                    Color = ConsoleColor.Black,
                    BackgroundColor = ConsoleColor.Red,
                });
        }
    }
}
