namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IOptions<TFieldType, TField>
        where TField : IField
    {
        public Func<IEnumerable<TFieldType>>? GetOptionsEvent { get; }
        public TField Options(Func<IEnumerable<TFieldType>> getOptions);
        public TField Options(IEnumerable<TFieldType> options);
    }
}
