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
            var form = new Form();
            form.Add()
                .TextField("What color do you pick?")
                .SetDataProcessingBeforeValidations(value =>
                {
                    if (value is null)
                        return null;

                    foreach (var key in colors.Keys)
                    {
                        if (value.Contains(key))
                            return key;
                    }

                    return value;
                })
                .SetValidation(value =>
                {
                    if (value is null)
                        return null;

                    if (!colors.ContainsKey(value))
                        return "It is not a color";

                    return null;
                })
                .SetDataProcessingAfterValidations(value =>
                {
                    if (value is null)
                        return null;

                    return colors[value];
                })
                .OnConfirm(value => text = value!);

            form.Run();
        }
    }
}
