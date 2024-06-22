using ImprovedConsole.Forms;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class MultiSelectSample
    {
        public static void Run()
        {
            IEnumerable<string> selectedColors = [];
            IEnumerable<string> confirmedColors = [];
            IEnumerable<int> selectedNumbers = [];
            IEnumerable<int> confirmedNumbers = [];

            Form form = new();
            string[] colors = ["red", "dark green", "dark blue"];
            form.Add()
                .MultiSelect()
                .Title("Which colors do you pick?")
                .Options(colors)
                .OnChange(value => selectedColors = value)
                .OnConfirm(value => confirmedColors = value);

            int[] numbers = [1, 2, 3];
            form.Add()
                .MultiSelect<int>()
                .Title("Which numbers do you pick?")
                .Required(false)
                .Default([3])
                .Selected([3])
                .Options(numbers)
                .OnChange(value => selectedNumbers = value)
                .OnConfirm(value => confirmedNumbers = value);

            form.Run();
        }
    }
}
