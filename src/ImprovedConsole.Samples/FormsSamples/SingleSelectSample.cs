using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.SingleSelects;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class SingleSelectSample
    {
        public static void Run()
        {
            string? selectedValue = null;
            string? confirmedValue = null;

            Form form = new();
            string[] colors = ["red", "green", "blue"];
            form.Add()
                .SingleSelect()
                .Title("What color do you pick?")
                .Options(colors)
                .OnChange(value => selectedValue = value)
                .OnConfirm(value => confirmedValue = value);

            form.Run();
        }
    }
}
