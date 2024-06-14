namespace ImprovedConsole.Forms.Fields
{
    public class InitialValue<T>(T value)
    {
        public T Value { get; set; } = value;
    }
}
