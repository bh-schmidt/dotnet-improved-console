namespace ImprovedConsole.Forms.Fields.Events
{
    public interface ISelectedValue<TFieldType, TField>
        where TField : IField
    {
        public Func<TFieldType>? GetSelectedValueEvent { get; }
        public TField Selected(Func<TFieldType> getValue);
        public TField Selected(TFieldType value);
    }
}
