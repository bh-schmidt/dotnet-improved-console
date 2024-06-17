namespace ImprovedConsole.Forms.Fields.Events
{
    public interface ISetValidation<TField>
        where TField : IField
    {
        public Func<string, string?>? ValidateEvent { get; set; }
        public TField Validation(Func<string, string?> validate);
    }
}
