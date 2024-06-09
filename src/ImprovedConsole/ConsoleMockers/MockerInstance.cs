namespace ImprovedConsole.ConsoleMockers
{
    public class MockerInstance : ConsoleInstance
    {
        private ConsoleUi consoleUi = new ConsoleUi();
        private ConsoleMockOptions options;
        private const ConsoleColor DefaultBackgroundColor = ConsoleColor.White;
        private const ConsoleColor DefaultForegroundColor = ConsoleColor.Black;
        private ConsoleColor backgroundColor = DefaultBackgroundColor;
        private ConsoleColor foregroundColor = DefaultForegroundColor;

        public MockerInstance(ConsoleMockOptions options)
        {
            this.options = options;
        }

        public IEnumerator<ConsoleKeyInfo>? ReadKeyEnumerator { get; set; }
        public IEnumerator<string?>? ReadLineEnumerator { get; set; }

        public string GetOutput() => consoleUi.GetOutput();

        public override ConsoleInstance Clear()
        {
            consoleUi.Clear();

            if (options.DisableConsole)
                return this;

            return base.Clear();
        }

        public override (int Left, int Top) GetCursorPosition()
        {
            if (options.DisableConsole)
                return consoleUi.GetCursorPosition();

            return base.GetCursorPosition();
        }

        public override int GetWindowWidth()
        {
            if (options.DisableConsole)
                return consoleUi.GetWindowWidth();

            return base.GetWindowWidth();
        }

        public override ConsoleInstance Write(object obj)
        {
            consoleUi.Write(obj);

            if (options.DisableConsole)
                return this;

            return base.Write(obj);
        }

        public override ConsoleInstance WriteLine()
        {
            consoleUi.WriteLine();

            if (options.DisableConsole)
                return this;

            return base.WriteLine();
        }

        public override ConsoleInstance WriteLine(object obj)
        {
            consoleUi.WriteLine(obj);

            if (options.DisableConsole)
                return this;

            return base.WriteLine(obj);
        }

        public override ConsoleKeyInfo ReadKey()
        {
            if (ReadKeyEnumerator?.MoveNext() == true)
                return ReadKeyEnumerator.Current;

            if (options.DisableConsole)
                return default;

            return base.ReadKey();
        }

        public override string? ReadLine()
        {
            if (ReadLineEnumerator?.MoveNext() == true)
            {
                WriteLine(ReadLineEnumerator.Current!);
                return ReadLineEnumerator.Current;
            }

            if (options.DisableConsole)
                return null;

            return base.ReadLine();
        }

        public override ConsoleKeyInfo ReadKey(bool intercept)
        {
            if (ReadKeyEnumerator?.MoveNext() == true)
                return ReadKeyEnumerator.Current;

            if (options.DisableConsole)
                return default;

            return base.ReadKey(intercept);
        }

        public override ConsoleInstance SetCursorPosition(int left, int top)
        {
            consoleUi.SetCursorPosition(left, top);

            if (options.DisableConsole)
                return this;

            return base.SetCursorPosition(left, top);
        }

        public override ConsoleInstance SetCursorVisibility(bool visible)
        {
            if (options.DisableConsole)
            {
                cursorVisible = visible;
                return this;
            }

            return base.SetCursorVisibility(visible);
        }

        public override ConsoleColor GetBackgroundColor()
        {
            if (options.DisableConsole)
                return backgroundColor;

            return base.GetBackgroundColor();
        }

        public override ConsoleColor GetForegroundColor()
        {
            if (options.DisableConsole)
                return foregroundColor;

            return base.GetForegroundColor();
        }

        public override ConsoleInstance SetBackgroundColor(ConsoleColor color)
        {
            if (options.DisableConsole)
            {
                backgroundColor = color;
                return this;
            }

            return base.SetBackgroundColor(color);
        }

        public override ConsoleInstance SetForegroundColor(ConsoleColor color)
        {
            if (options.DisableConsole)
            {
                foregroundColor = color;
                return this;
            }

            return base.SetForegroundColor(color);
        }
    }
}
