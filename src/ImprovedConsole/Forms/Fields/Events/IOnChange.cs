namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IOnChange<TFieldType, TField>
        where TField : IField
    {
        public Action<TFieldType>? OnChangeEvent { get; set; }
        public TField OnChange(Action<TFieldType> callback);
    }
}
