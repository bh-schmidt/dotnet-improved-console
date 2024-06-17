namespace ImprovedConsole.Forms.Fields.Events
{
    public interface IOnConfirm<TFieldType, TField>
        where TField : IField
    {
        public Action<TFieldType>? OnConfirmEvent { get; set; }
        public TField OnConfirm(Action<TFieldType> callback);
    }
}
