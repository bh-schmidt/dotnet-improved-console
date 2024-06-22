using ImprovedConsole.Forms;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class SingleSelectSample
    {
        public static void Run()
        {
            string? selectedColor = null;
            string? confirmedColor = null;
            int? selectedNumber = null;
            int? confirmedNumber = null;

            Form form = new();
            string[] colors = ["red", "green", "blue"];
            form.Add()
                .SingleSelect()
                .Title("Which color do you pick?")
                .Options(colors)
                .OnChange(value => selectedColor = value)
                .OnConfirm(value => confirmedColor = value);

            int[] numbers = [1, 2, 3];
            form.Add()
                .SingleSelect<int>()
                .Title("Which number do you pick?")
                .Required(false)
                .Default(3)
                .Selected(3)
                .Options(numbers)
                .OnChange(value => selectedNumber = value)
                .OnConfirm(value => confirmedNumber = value);

            form.Run();
        }
    }
}
