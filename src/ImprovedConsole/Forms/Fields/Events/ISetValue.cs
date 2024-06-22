namespace ImprovedConsole.Forms.Fields.Events
{
    public interface ISetValue<TFieldType, TField>
        where TField : IField
    {
        public Func<TFieldType>? GetExternalValueEvent { get; }
        public TField Set(Func<TFieldType> getValue);
        public TField Set(TFieldType value);
    }
}
