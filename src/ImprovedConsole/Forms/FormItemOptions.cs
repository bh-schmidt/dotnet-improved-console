using ImprovedConsole.Forms.Fields;

namespace ImprovedConsole.Forms
{
    public class FormItemOptions
    {
        public Func<bool> CanExecute { get; set; } = () => true;
        public IField? DependsOn { get; set; }
    }
}
