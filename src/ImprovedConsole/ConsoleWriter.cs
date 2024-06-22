namespace ImprovedConsole
{
    public static class ConsoleWriter
    {
        public static ConsoleInstance Instance { get; set; } = new ConsoleInstance();

        public static ConsoleInstance Clear() => Instance.Clear();
        public static ConsoleInstance ClearLine() => Instance.ClearLine();
        public static ConsoleInstance ClearLines(int startIndex, int endIndex) => Instance.ClearLines(startIndex, endIndex);
        public static (int Left, int Top) GetCursorPosition() => Instance.GetCursorPosition();
        public static bool GetCursorVisibility() => Instance.GetCursorVisibility();
        public static ConsoleKeyInfo ReadKey() => Instance.ReadKey();
        public static string? ReadLine() => Instance.ReadLine();
        public static ConsoleKeyInfo ReadKey(bool intercept) => Instance.ReadKey(intercept);
        public static ConsoleColor GetBackgroundColor() => Instance.GetBackgroundColor();
        public static ConsoleColor GetForegroundColor() => Instance.GetForegroundColor();
        public static ConsoleColor GetDefaultBackgroundColor() => Instance.GetBackgroundColor();
        public static ConsoleColor GetDefaultForegroundColor() => Instance.GetForegroundColor();
        public static ConsoleInstance SetBackgroundColor(ConsoleColor color) => Instance.SetBackgroundColor(color);
        public static ConsoleInstance SetCursorPosition(int left, int top) => Instance.SetCursorPosition(left, top);
        public static ConsoleInstance SetCursorVisibility(bool visible) => Instance.SetCursorVisibility(visible);
        public static ConsoleInstance SetForegroundColor(ConsoleColor color) => Instance.SetForegroundColor(color);
        public static ConsoleInstance SetIn(TextReader newIn) => Instance.SetIn(newIn);
        public static ConsoleInstance SetOut(TextWriter newOut) => Instance.SetOut(newOut);
        public static ConsoleInstance Write(object? obj) => Instance.Write(obj);
        public static ConsoleInstance WriteLine(object? obj) => Instance.WriteLine(obj);
        public static ConsoleInstance WriteLine() => Instance.WriteLine();
        public static int GetWindowWidth() => Instance.GetWindowWidth();

        public static bool CanSetCursorVisibility() => Instance.CanSetCursorVisibility();
        public static bool CanSetCursorPosition() => Instance.CanSetCursorPosition();
        public static bool CanGetWindowWidth() => Instance.CanGetWindowWidth();
    }
}
