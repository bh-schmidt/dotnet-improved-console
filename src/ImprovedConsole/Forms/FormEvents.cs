using ImprovedConsole.Forms.Fields;

namespace ImprovedConsole.Forms
{
    public class FormEvents
    {
        public event Action ReprintRequested = () => { };
        public event Action<IField> ResetRequested = (field) => { };

        public void Reprint() => ReprintRequested();
        public void Reset(IField field) => ResetRequested(field);
    }
}
