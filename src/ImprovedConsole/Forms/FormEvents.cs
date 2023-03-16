using ImprovedConsole.Forms.Fields;

namespace ImprovedConsole.Forms
{
    public class FormEvents
    {
        public event Action ReprintRequested = () => { };

        public void Reprint() => ReprintRequested();
    }
}
