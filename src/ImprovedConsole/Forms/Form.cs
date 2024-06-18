﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ImprovedConsole.Forms
{
    public class Form
    {
        private readonly FormEvents formEvents;
        private readonly FormOptions options;
        private readonly ConcurrentItemsSync formItemBox;
        private readonly HashSet<object> fieldIds;

        private FormItem? confirmationField;
        private FormItem? fieldSelector;

        private bool isRunning;
        private bool isFinished;
        private bool runningConfirmation;

        public Form() : this(new FormOptions())
        {
        }

        public Form(FormOptions options)
        {
            this.options = options;
            formItemBox = new();
            fieldIds = [];

            formEvents = new FormEvents();
            formEvents.ReprintEvent += Reprint;
        }

        public FormItem Add()
        {
            return Add(Guid.NewGuid());
        }

        public FormItem Add(FormItemOptions options)
        {
            return Add(Guid.NewGuid(), options);
        }

        public FormItem Add(object fieldId)
        {
            return Add(fieldId, new());
        }

        public FormItem Add(object fieldId, FormItemOptions options)
        {
            if (TryAdd(fieldId, options, out var item))
                return item;

            throw new ArgumentException("The field id already exists.");
        }

        public bool TryAdd(object fieldId, out FormItem? item)
        {
            return TryAdd(fieldId, new(), out item);
        }

        public bool TryAdd(object fieldId, FormItemOptions options, [NotNullWhen(true)] out FormItem? item)
        {
            ArgumentNullException.ThrowIfNull(fieldId, nameof(fieldId));

            item = null;

            lock (fieldIds)
            {
                if (fieldIds.Contains(fieldId))
                    return false;
            }

            item = new(formEvents, options ?? new FormItemOptions())
            {
                Id = fieldId
            };

            formItemBox.Add(item);

            lock (fieldIds)
            {
                fieldIds.Add(fieldId);
            }

            return true;
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

            foreach (FormItem formItem in formItemBox.GetInstance())
                formItem.Reset();
        }

        private void RunInternal()
        {
            SetConfirmationForms();

            do
            {
                RunItems();
                PrintAnswers();

                if (formItemBox.GetInstance().Any(e => e.Finished))
                {
                    runningConfirmation = true;
                    confirmationField?.Run();
                    if (!isFinished)
                    {
                        PrintAnswers();
                        fieldSelector?.Run();
                    }
                    runningConfirmation = false;
                }
            } while (!isFinished);
        }

        private void RunItems()
        {
            while (true)
            {
                var formItems = formItemBox.GetInstance();
                FormItem? item = formItems.FirstOrDefault(e => !e.Finished && e.Options.Condition());

                if (item is null)
                    break;

                var sameAnswer = item.Run();

                if (!sameAnswer)
                {
                    var dependencies = formItems.Where(e =>
                        e.Options.Dependencies is not null &&
                        e.Options.Dependencies.Contains(item.Field!));

                    var finishedResets = formItems.Where(e =>
                        e.Finished &&
                        !e.Options.Condition());

                    var resetItems = dependencies
                        .Concat(finishedResets)
                        .Distinct();

                    ResetItems(resetItems);
                }
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
                .TextOption()
                .Title("Do you want to edit something?")
                .Options(possibilities)
                .OnConfirm(value =>
                {
                    isFinished = value == "n";
                    fieldSelector.Reset();
                });

            fieldSelector
                .TextOption<string>()
                .Title("Type the number of the field you want to edit")
                .Options(() =>
                {
                    var itemNumbers = formItemBox.GetInstance()
                        .Where(e => e.Finished)
                        .Select((e, i) => i + 1)
                        .Select(e => e.ToString());

                    return itemNumbers;
                })
                .OnConfirm(value =>
                {
                    if (value is null)
                    {
                        confirmationField.Reset();
                        return;
                    }

                    int index = int.Parse(value) - 1;
                    FormItem item = formItemBox.GetInstance()
                        .Where(e => e.Options.Condition())
                        .Skip(index)
                        .Take(1)
                        .First();

                    item.SoftReset();
                });
        }

        private void Reprint()
        {
            if (!formItemBox.GetInstance().Any(e => e.Finished && e.Options.Condition()))
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
            IEnumerable<FormItem> finishedItems = formItemBox
                .GetInstance()
                .Where(e => e.Finished && e.Options.Condition());

            foreach (FormItem? item in finishedItems)
            {

                stringBuilder
                    .Append($"{{color:{ConsoleColor.Blue}}}");

                var spacingBuilder = new StringBuilder();

                if (isFinished || runningConfirmation)
                    spacingBuilder.Append(itemNumber);
                else
                    spacingBuilder.Append(' ');

                spacingBuilder.Append("- ");

                itemNumber++;

                var answer = item.GetFormattedAnswer(spacingBuilder.Length, options);
                stringBuilder
                    .Append(spacingBuilder)
                    .Append(answer)
                    .AppendLine();
            }

            string message = stringBuilder.ToString();
            ConsoleWriter.Clear();
            Message.WriteLine(message);
        }

        private void ResetItems(IEnumerable<FormItem> items)
        {
            foreach (FormItem? item in items)
                item.Reset();
        }
    }
}
