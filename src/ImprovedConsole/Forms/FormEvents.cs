namespace ImprovedConsole.Forms
{
    public class FormEvents
    {
        public event Action ReprintEvent = () => { };

        public void Reprint() => ReprintEvent();
    }
}
