using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextFields;
using ImprovedConsole.Forms.Fields.TextOptions;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class FullFormSample
    {
        public static void Run()
        {
            string? procceed;
            string? name;
            IEnumerable<string> favoriteColors;
            string? age;

            var form = new Form();

            TextOptionOptions options = new TextOptionOptions { Required = true };
            form.Add()
                .TextOption("Do you want to procceed?", new[] { "y", "n" }, options)
                .OnConfirm(value => procceed = value);

            TextFieldOptions textFieldOptions = new TextFieldOptions { Required = true };
            form.Add()
                .TextField("What is your name?", textFieldOptions)
                .OnConfirm(value => name = value);

            form.Add()
                .MultiSelect("What color do you like more?", new[] { "red", "green", "blue" })
                .OnConfirm(values =>
                {
                    favoriteColors = values.Select(e => e.Value);
                });

            form.Add()
                .SingleSelect("Which age range are you in?", new[] { "< 18", "18 - 30", "30 <" })
                .OnConfirm(value => age = value?.Value);

            form.Run();
        }
    }
}
