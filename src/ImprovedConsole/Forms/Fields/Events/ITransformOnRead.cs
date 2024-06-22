namespace ImprovedConsole.Forms.Fields.Events
{
    public interface ITransformOnRead<TField>
        where TField : IField
    {
        public Func<string, string>? TransformOnReadEvent { get; }
        public TField TransformOnRead(Func<string, string> transform);
    }
}
