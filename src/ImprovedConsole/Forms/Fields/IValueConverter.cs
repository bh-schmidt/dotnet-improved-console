namespace ImprovedConsole.Forms.Fields
{
    public interface IValueConverter<TTarget>
    {
        TTarget? Convert(string? value);
    }
}
