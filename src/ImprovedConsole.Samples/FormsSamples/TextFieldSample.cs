using ImprovedConsole.Forms;

namespace ImprovedConsole.Samples.FormsSamples
{
    public class TextFieldSample
    {
        public static void Run()
        {
            Dictionary<string, string> colors = new()
            {
                {"red", "#cc0000" },
                {"green", "#00ff00" },
                {"blue", "#0000ff" }
            };

            string text;
            Form form = new();
            form.Add()
                .TextField()
                .Title("What color do you pick?")
                .TransformOnRead(value =>
                {
                    foreach (string key in colors.Keys)
                    {
                        if (value.Contains(key))
                            return key;
                    }

                    return value;
                })
                .Validation(value =>
                {
                    if (!colors.ContainsKey(value))
                        return "It is not a color";

                    return null;
                })
                .TransformOnValidate(value =>
                {
                    return colors[value];
                })
                .OnConfirm(value => text = value!)
                .ValidateField();

            form.Run();
        }
    }
}
