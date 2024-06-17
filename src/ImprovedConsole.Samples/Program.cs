using ImprovedConsole.Forms;
using ImprovedConsole.Samples.CommandRunnersSamples;
using ImprovedConsole.Samples.FormsSamples;
using ImprovedConsole.Samples.LoggerSamples;

Form form = new(
    new()
    {
        ShowConfirmationForms = false,
    });

string[] sampleList = [
    "commands",
    "command-groups",
    "full-form",
    "text-field",
    "single-select",
    "colored-logger"
];

string? selected = null;

form
    .Add()
    .SingleSelect()
    .Title("Which sample do you want to run?")
    .Options(sampleList)
    .OnConfirm(item =>
    {
        selected = item;
    });

form.Run();

switch (selected)
{
    case "commands":
        await CommandsSample.RunAsync();
        break;

    case "command-groups":
        await CommandGroupSample.RunAsync();
        break;

    case "full-form":
        FullFormSample.Run();
        break;

    case "text-field":
        TextFieldSample.Run();
        break;

    case "single-select":
        SingleSelectSample.Run();
        break;

    case "colored-logger":
        ColoredLoggerSample.Run();
        break;

    default:
        break;
}
