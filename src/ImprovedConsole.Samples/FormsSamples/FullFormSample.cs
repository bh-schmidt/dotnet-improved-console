using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextFields;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class FullFormSample
    {
        public static void Run()
        {
            string? proceed;
            string? name;
            IEnumerable<string> favoriteColors;
            string? age;

            Form form = new();

            string[] confirmations = ["y", "n"];
            form.Add()
                .TextOption()
                .Title("Do you want to proceed?")
                .Options(confirmations)
                .OnConfirm(value => proceed = value)
                .ValidateField();

            form.Add()
                .TextField()
                .Required(true)
                .Title("What is your name?")
                .OnConfirm(value => name = value);

            string[] colors = ["red", "green", "blue"];
            form.Add()
                .MultiSelect()
                .Title("What color do you like more?")
                .Options(colors)
                .OnConfirm(values =>
                {
                    favoriteColors = values;
                });

            string[] ages = ["< 18", "18 - 30", "30 <"];
            form.Add()
                .SingleSelect()
                .Title("Which age range are you in?")
                .Options(ages)
                .OnConfirm(value => age = value);

            form.Run();
        }
    }
}
