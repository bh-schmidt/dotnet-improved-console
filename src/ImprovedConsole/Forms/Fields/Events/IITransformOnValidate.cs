namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IITransformOnValidate<TField>
        where TField : IField
    {
        public Func<string, string>? TransformOnValidateEvent { get; set; }
        public TField TransformOnValidate(Func<string, string> transform);
    }
}
