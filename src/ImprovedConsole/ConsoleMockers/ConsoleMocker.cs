namespace ImprovedConsole.ConsoleMockers
{
    public class ConsoleMock : IDisposable
    {
        private readonly MockerInstance instance;
        private readonly ConsoleInstance realInstance;
        private readonly ConsoleMockerSetup setup;

        public ConsoleMock() : this(new ConsoleMockOptions())
        {
        }

        public ConsoleMock(ConsoleMockOptions options)
        {
            instance = new MockerInstance(options);
            realInstance = ConsoleWriter.Instance;

            ConsoleWriter.Instance = instance;
            setup = new ConsoleMockerSetup(instance);
        }

        public void Dispose()
        {
            if (realInstance.GetType() == typeof(ConsoleInstance))
            {
                ConsoleWriter.Instance = realInstance;
                return;
            }

            ConsoleWriter.Instance = new ConsoleInstance();

            GC.SuppressFinalize(this);
        }

        public string GetOutput() => instance.GetOutput();

        public ConsoleMockerSetup Setup() => setup;
    }

    public class ConsoleMockOptions
    {
        public bool DisableConsole { get; set; } = true;
    }
}
