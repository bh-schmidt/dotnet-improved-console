namespace ImprovedConsole.ConsoleMockers
{
    public class ConsoleMock : IDisposable
    {
        private readonly MockerInstance instance;
        private ConsoleInstance realInstance;
        private readonly ConsoleMockerSetup setup;
        private readonly ConsoleMockOptions options;

        public ConsoleMock() : this(new ConsoleMockOptions())
        {
        }

        public ConsoleMock(ConsoleMockOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
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
        }

        public string GetOutput() => instance.GetOutput();

        public ConsoleMockerSetup Setup() => setup;
    }

    public class ConsoleMockOptions
    {
        public bool DisableConsole { get; set; } = true;
    }
}
