using ImprovedConsole.Forms;
using ImprovedConsole.Samples.CommandRunnersSamples;
using ImprovedConsole.Samples.FormsSamples;
using ImprovedConsole.Samples.LoggerSamples;

var form = new Form(new()
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
    .SingleSelect("Wich sample do you want to run?", sampleList)
    .OnConfirm(item =>
    {
        selected = item?.Value;
    });

form.Run();

switch (selected)
{
    case "commands":
        CommandsSample.Run();
        break;

    case "command-groups":
        CommandGroupSample.Run();
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
