
namespace ImprovedConsole.Forms.Fields
{
    public interface IField
    {
        bool IsEditing { get; }
        Func<string> GetTitle { get; }
        public IFieldAnswer? Answer { get; }
        public bool Finished { get; }
        public Func<bool> ConditionDelegate { get; }
        public HashSet<IField> Dependencies { get; }

        IFieldAnswer Run();
        void Reset();
        void SetEdition();
    }
}
