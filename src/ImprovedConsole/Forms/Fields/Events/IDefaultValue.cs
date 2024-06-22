namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IDefaultValue<TFieldType, TField>
        where TField : IField
    {
        public Func<TFieldType>? GetDefaultValueEvent { get; }
        public TField Default(Func<TFieldType> getValue);
        public TField Default(TFieldType value);
    }
}
