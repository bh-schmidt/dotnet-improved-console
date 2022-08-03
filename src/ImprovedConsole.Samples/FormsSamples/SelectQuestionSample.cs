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

            var form = new Form();
            form.Add()
                .SingleSelect("What color do you pick?", new[] { "red", "green", "blue" })
                .OnChange(value => selectedValue = value)
                .OnConfirm(value => confirmedValue = value);

            form.Run();
        }
    }
}
