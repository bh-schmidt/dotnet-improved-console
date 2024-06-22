using ImprovedConsole.Extensions;

namespace ImprovedConsole.Forms.Fields.TextFields
{
    public class TextFieldRunner<TFieldType>(TextField<TFieldType> textField, string title, bool required, TFieldType? defaultValue)
    {
        public TextFieldAnswer<TFieldType> Run()
        {
            TFieldType? value = default;
            string? error = null;

            while (true)
            {
                textField.FormEvents.Reprint();
                string? strValue = Read(title, error);

                if (string.IsNullOrEmpty(strValue))
                {
                    if (required)
                    {
                        error = "This field is required.";
                        continue;
                    }

                    if (textField.GetDefaultValueEvent is not null)
                    {
                        value = defaultValue;
                        break;
                    }

                    value = default;
                    break;
                }

                strValue = textField.TransformOnReadEvent?.Invoke(strValue) ?? strValue;

                error = textField.ValidateEvent?.Invoke(strValue);
                if (!error.IsNullOrEmpty())
                    continue;

                strValue = textField.TransformOnValidateEvent?.Invoke(strValue) ?? strValue;

                if (!textField.TryConvertFromString(strValue, out var conversion))
                {
                    error = "Could not convert the value.";
                    continue;
                }

                value = conversion;
                break;
            }

            textField.OnConfirmEvent?.Invoke(value);
            return new TextFieldAnswer<TFieldType>(textField, title, value);
        }

        private static string? Read(string title, string? error)
        {
            string? line;
            Message.WriteLine(title);

            if (error is not null && error.Length > 0)
                Message.WriteLine("{color:red} * " + error);

            line = ConsoleWriter.ReadLine();
            return line;
        }

    }
}
