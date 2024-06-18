namespace ImprovedConsole.Forms.Fields
{
    public interface IValueConverter<TTarget>
    {
        TTarget ConvertFromString(string value);
        string ConvertToString(TTarget value);
    }
}
