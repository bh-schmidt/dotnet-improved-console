using ImprovedConsole.Forms.Fields.TextOptions;
using System.Text;

namespace ImprovedConsole.Forms
{
    public class Form
    {
        private readonly FormEvents formEvents;
        private readonly FormOptions options;
        private readonly LinkedList<FormItem> formItems;

        private FormItem? confirmationField;
        private FormItem? fieldSelector;

        private bool isRunning;
        private bool isFinished;

        public Form() : this(new FormOptions())
        {
        }

        public Form(FormOptions options)
        {
            this.options = options;
            formItems = new LinkedList<FormItem>();

            formEvents = new FormEvents();
            formEvents.ReprintRequested += Reprint;
        }

        public FormItem Add(FormItemOptions? options = null)
        {
            if (isRunning)
                throw new Exception("Can't add new fields while running.");

            FormItem item = new(formEvents, options ?? new FormItemOptions());
            formItems.AddLast(item);

            return item;
        }

        public void Run()
        {
            try
            {
                isRunning = true;
                RunInternal();
                isRunning = false;
            }
            catch
            {
                isRunning = false;
                throw;
            }
        }

        public void Clear()
        {
            if (isRunning)
                throw new Exception("Can't clear while running.");

            foreach (FormItem item in formItems)
                Reset(item);
        }

        private void RunInternal()
        {
            SetConfirmationForms();

            do
            {
                RunItems();
                PrintAnswers();

                if (formItems.Any(e => e.Finished))
                {
                    confirmationField?.Run();
                    if (!isFinished)
                    {
                        PrintAnswers();
                        fieldSelector?.Run();
                    }
                }
            } while (!isFinished);
        }

        private void RunItems()
        {
            while (formItems.Any(e => !e.Finished && e.Options.Condition()))
            {
                FormItem? item = formItems.FirstOrDefault(e => !e.Finished && e.Options.Condition());
                if (item is null)
                    break;

                Reprint();
                item.Run();
            }

            isFinished = true;
        }

        private void SetConfirmationForms()
        {
            if (!options.ShowConfirmationForms)
                return;

            confirmationField = new FormItem(formEvents, new FormItemOptions());
            fieldSelector = new FormItem(formEvents, new FormItemOptions());

            string[] possibilities = ["y", "n"];
            confirmationField
                .TextOption("Do you want to edit something?", possibilities, new TextOptionOptions { Required = true })
                .OnConfirm(value =>
                {
                    isFinished = value == "n";
                    fieldSelector.Reset();
                });

            IEnumerable<string> availableOptions = Enumerable.Range(1, formItems.Count).Select(e => e.ToString());
            fieldSelector
                .TextOption("Type the number of the field you want to edit", availableOptions, new TextOptionOptions { ShowOptions = false })
                .OnConfirm(value =>
                {
                    if (value is null)
                    {
                        confirmationField.Reset();
                        return;
                    }

                    int index = int.Parse(value) - 1;
                    FormItem item = formItems
                        .Where(e => e.Options.Condition())
                        .Skip(index)
                        .Take(1)
                        .First();

                    Reset(item);
                });
        }

        private void Reprint()
        {
            if (!formItems.Any(e => e.Finished && e.Options.Condition()))
            {
                ConsoleWriter.Clear();
                return;
            }

            PrintAnswers();
        }

        private void PrintAnswers()
        {
            StringBuilder stringBuilder = new();

            int itemNumber = 1;
            IEnumerable<FormItem> finishedItems = formItems.Where(e => e.Finished && e.Options.Condition());

            foreach (FormItem? item in finishedItems)
            {
                string answer = item.GetFormattedAnswer(options);

                stringBuilder
                    .Append($"{{color:{ConsoleColor.Blue}}}")
                    .Append(itemNumber)
                    .Append("- ");

                itemNumber++;

                stringBuilder.AppendLine(answer);
            }

            string message = stringBuilder.ToString();
            ConsoleWriter.Clear();
            Message.WriteLine(message);
        }

        private void Reset(FormItem formItem)
        {
            formItem.Reset();

            IEnumerable<FormItem> dependencies = formItems
                .Where(e => e.Options.Dependencies is not null && e.Options.Dependencies.Contains(formItem.Field!));

            foreach (FormItem? dependency in dependencies)
                dependency.Reset();
        }
    }
}
