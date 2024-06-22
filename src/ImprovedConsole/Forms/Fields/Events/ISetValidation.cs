namespace ImprovedConsole.Forms.Fields.Events
{
    public interface ISetValidation<TField>
        where TField : IField
    {
        public Func<string, string?>? ValidateEvent { get; }
        public TField Validation(Func<string, string?> validate);
    }
}
