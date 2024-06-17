namespace ImprovedConsole.Forms.Fields.Events
{
    public interface ISetValue<TFieldType, TField>
        where TField : IField
    {
        public Func<TFieldType>? GetValueToSetEvent { get; set; }
        public TField Set(Func<TFieldType> getValue);
        public TField Set(TFieldType value);
    }
}
