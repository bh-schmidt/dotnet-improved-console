namespace ImprovedConsole
{
    public class ConsoleInstance
    {
        protected bool cursorVisible = true;

        private readonly ConsoleColor? defaultBackgroundColor;
        private readonly ConsoleColor? defaultForegroundColor;

        public ConsoleInstance()
        {
            try
            {
                defaultForegroundColor = Console.ForegroundColor;
                defaultBackgroundColor = Console.BackgroundColor;
            }
            catch { }
        }

        public virtual ConsoleInstance Clear()
        {
            Console.Clear();
            return this;
        }

        public virtual ConsoleInstance ClearLine()
        {
            (_, int Top) = GetCursorPosition();
            ClearLines(Top, Top);
            return this;
        }

        public virtual ConsoleInstance ClearLines(int fromLine, int toLine)
        {
            bool visible = GetCursorVisibility();
            SetCursorVisibility(false);

            (int Left, int Top) = GetCursorPosition();
            for (int topIndex = fromLine; topIndex <= toLine; topIndex++)
                for (int leftIndex = 0; leftIndex < GetWindowWidth(); leftIndex++)
                    Clear(topIndex, leftIndex);

            SetCursorPosition(Left, Top);
            SetCursorVisibility(visible);

            return this;
        }

        public virtual bool GetCursorVisibility()
        {
            return cursorVisible;
        }

        public virtual ConsoleInstance SetCursorVisibility(bool visible)
        {
            cursorVisible = visible;
            Console.CursorVisible = visible;
            return this;
        }

        private void Clear(int topIndex, int leftIndex)
        {
            SetCursorPosition(leftIndex, topIndex);
            Write(' ');
        }

        public virtual ConsoleInstance ClearBox((int Left, int Top) from, (int Left, int Top) to)
        {
            if (from.Left < 0 || from.Top < 0)
                throw new ArgumentOutOfRangeException(nameof(from), "The from position can't be lower than (0, 0).");

            if (to.Left >= Console.WindowWidth)
                throw new ArgumentOutOfRangeException(nameof(to), "The to position can't be greater than the window width.");

            (int Left, int Top) = GetCursorPosition();

            for (int topIndex = from.Top; topIndex < to.Top + 1; topIndex++)
                for (int leftIndex = from.Left; leftIndex < to.Left + 1; leftIndex++)
                {
                    SetCursorPosition(leftIndex, topIndex);
                    Write(' ');
                }

            SetCursorPosition(Left, Top);

            return this;
        }

        public virtual int GetWindowWidth() => Console.WindowWidth;

        public virtual (int Left, int Top) GetCursorPosition() => Console.GetCursorPosition();

        public virtual ConsoleKeyInfo ReadKey() => Console.ReadKey();
        public virtual string? ReadLine() => Console.ReadLine();

        public virtual ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

        public virtual ConsoleColor GetBackgroundColor()
        {
            return Console.BackgroundColor;
        }

        public virtual ConsoleColor GetForegroundColor()
        {
            return Console.ForegroundColor;
        }

        public virtual ConsoleInstance SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(left, top);
            return this;
        }

        public virtual ConsoleInstance SetBackgroundColor(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            return this;
        }

        public virtual ConsoleInstance SetForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
            return this;
        }

        public virtual ConsoleInstance SetIn(TextReader newIn)
        {
            Console.SetIn(newIn);
            return this;
        }

        public virtual ConsoleInstance SetOut(TextWriter newOut)
        {
            Console.SetOut(newOut);
            return this;
        }

        public virtual ConsoleInstance Write(object obj)
        {
            Console.Write(obj);
            return this;
        }

        public virtual ConsoleInstance WriteLine(object obj)
        {
            Console.WriteLine(obj);
            return this;
        }

        public virtual ConsoleInstance WriteLine()
        {
            Console.WriteLine();
            return this;
        }

        public virtual ConsoleColor GetDefaultBackgroundColor() => defaultBackgroundColor ?? throw new NotSupportedException();

        public virtual ConsoleColor GetDefaultForegroundColor() => defaultForegroundColor ?? throw new NotSupportedException();
    }
}
