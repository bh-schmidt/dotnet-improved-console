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

            TextOptionOptions options = new() { Required = true };
            string[] confirmations = ["y", "n"];
            form.Add()
                .TextOption("Do you want to proceed?", confirmations, options)
                .OnConfirm(value => proceed = value);

            TextFieldOptions textFieldOptions = new() { Required = true };
            form.Add()
                .TextField("What is your name?", textFieldOptions)
                .OnConfirm(value => name = value);

            string[] colors = ["red", "green", "blue"];
            form.Add()
                .MultiSelect("What color do you like more?", colors)
                .OnConfirm(values =>
                {
                    favoriteColors = values.Select(e => e.Value);
                });

            string[] ages = ["< 18", "18 - 30", "30 <"];
            form.Add()
                .SingleSelect("Which age range are you in?", ages)
                .OnConfirm(value => age = value?.Value);

            form.Run();
        }
    }
}
