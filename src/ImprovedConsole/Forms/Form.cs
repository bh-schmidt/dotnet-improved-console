using ImprovedConsole.Forms.Fields;
using ImprovedConsole.Forms.Fields.OptionSelectors;
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

            var item = new FormItem(formEvents, options ?? new FormItemOptions());
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

        private void RunInternal()
        {
            SetConfirmationForms();

            do
            {
                RunItems();
                PrintAnswers(true);

                confirmationField?.Run();
                if (!isFinished)
                {
                    PrintAnswers(true);
                    fieldSelector?.Run();
                }
            } while (!isFinished);
        }

        private void RunItems()
        {
            while (formItems.Any(e => !e.Finished && e.Options.Condition()))
            {
                var item = formItems.FirstOrDefault(e => !e.Finished && e.Options.Condition());
                if (item is null)
                    break;

                Reprint();
                item.Run();
            }
        }

        private void SetConfirmationForms()
        {
            if (!options.ShowConfirmationForms)
                return;

            confirmationField = new FormItem(formEvents, new FormItemOptions());
            fieldSelector = new FormItem(formEvents, new FormItemOptions());

            confirmationField
                .OptionSelector("Do you want to edit something?", new[] { "yes", "no" }, new OptionSelectorsOptions { Required = true })
                .OnConfirm(value =>
                {
                    isFinished = value != "yes";
                    fieldSelector.Reset();
                });

            var availableOptions = Enumerable.Range(1, formItems.Count).Select(e => e.ToString());
            fieldSelector
                .OptionSelector("Type the number of the field you want to edit", availableOptions, new OptionSelectorsOptions { ShowOptions = false })
                .OnConfirm(value =>
                {
                    if (value is null)
                    {
                        confirmationField.Reset();
                        return;
                    }

                    var index = int.Parse(value) - 1;
                    var item = formItems
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

            PrintAnswers(false);
        }

        private void PrintAnswers(bool addNumeration)
        {
            StringBuilder stringBuilder = new();

            int itemNumber = 1;
            var itemNumberColor = ConsoleWriter.GetForegroundColor();
            var finishedItems = formItems.Where(e => e.Finished && e.Options.Condition());

            foreach (var item in finishedItems)
            {
                var answer = item.GetFormattedAnswer(options);

                if (addNumeration)
                {
                    stringBuilder
                        .Append($"{{color:{ConsoleColor.Blue}}}")
                        .Append("(")
                        .Append(itemNumber)
                        .Append(") ");

                    itemNumber++;
                }

                stringBuilder.AppendLine(answer);
            }

            var message = stringBuilder.ToString();
            ConsoleWriter.Clear();
            Message.WriteLine(message);
        }

        private void Reset(FormItem formItem)
        {
            formItem.Reset();

            var dependencies = formItems
                .Where(e => e.Options.Dependencies is not null && e.Options.Dependencies.Contains(formItem.Field));

            foreach (var dependency in dependencies)
                dependency.Reset();
        }
    }
}
