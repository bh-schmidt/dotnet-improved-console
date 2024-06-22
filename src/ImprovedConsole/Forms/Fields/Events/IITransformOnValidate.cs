namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IITransformOnValidate<TField>
        where TField : IField
    {
        public Func<string, string>? TransformOnValidateEvent { get; }
        public TField TransformOnValidate(Func<string, string> transform);
    }
}
