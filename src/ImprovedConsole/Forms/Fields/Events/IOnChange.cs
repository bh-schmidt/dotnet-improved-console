namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IOnChange<TFieldType, TField>
        where TField : IField
    {
        public Action<TFieldType>? OnChangeEvent { get; }
        public TField OnChange(Action<TFieldType> callback);
    }
}
