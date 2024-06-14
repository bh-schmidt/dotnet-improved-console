using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class SingleSelectSample
    {
        public static void Run()
        {
            PossibilityItem? selectedValue = null;
            PossibilityItem? confirmedValue = null;

            Form form = new();
            string[] colors = ["red", "green", "blue"];
            form.Add()
                .SingleSelect("What color do you pick?", colors)
                .OnChange(value => selectedValue = value)
                .OnConfirm(value => confirmedValue = value);

            form.Run();
        }
    }
}
