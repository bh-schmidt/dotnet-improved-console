namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IOnReset<TFieldType, TField>
        where TField : IField
    {
        public Action<TFieldType>? OnResetEvent { get; }
        public TField OnReset(Action<TFieldType> callback);
    }
}
