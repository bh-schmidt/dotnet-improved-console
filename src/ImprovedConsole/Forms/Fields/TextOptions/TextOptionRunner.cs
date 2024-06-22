using ImprovedConsole.Forms.Fields.Events;

namespace ImprovedConsole.Forms.Fields.TextOptions
{
    public class TextOptionRunner<TFieldType>(
        TextOption<TFieldType> textOption,
        string title,
        bool required,
        IEnumerable<TFieldType> options,
        TFieldType? defaultValue)
    {
        public TextOptionAnswer<TFieldType> Run()
        {
            string? error = null;
            TFieldType? value;

            while (true)
            {
                textOption.FormEvents.Reprint();
                var readValue = Read(title, options, error);

                if (string.IsNullOrEmpty(readValue))
                {
                    if (required)
                    {
                        error = "This field is required.";
                        continue;
                    }

                    if (textOption.GetDefaultValueEvent is not null)
                    {
                        value = defaultValue;
                        break;
                    }

                    value = default;
                    break;
                }

                if (!textOption.TryConvertFromString(readValue, out value))
                {
                    error = "Could not convert the value.";
                    continue;
                }

                if (!options.Contains(value))
                {
                    error = "Invalid option.";
                    continue;
                }

                break;
            }

            textOption.OnConfirmEvent?.Invoke(value);

            return new TextOptionAnswer<TFieldType>(textOption, title, value);
        }

        private string? Read(string title, IEnumerable<TFieldType> options, string? error)
        {
            ConsoleColor color = ConsoleWriter.GetForegroundColor();

            Message.Write(title);

            IEnumerable<string?> stringOptions = options.Select(textOption.ConvertToStringDelegate);
            string optionsText = $"({string.Join("/", stringOptions)})";
            ConsoleWriter
                .Write(' ')
                .Write(optionsText)
                .WriteLine();

            if (error is not null && error.Length > 0)
                Message.WriteLine("{color:red} * " + error);

            return ConsoleWriter.ReadLine();
        }
    }
}
