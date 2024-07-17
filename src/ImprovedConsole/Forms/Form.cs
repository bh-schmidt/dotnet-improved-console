using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ImprovedConsole.Forms
{
    public class Form
    {
        private readonly FormEvents formEvents;
        private readonly FormOptions options;

        private readonly ConcurrentItemsSync<FormSection> sectionsBox;
        private readonly HashSet<object> sectionsIds;

        private bool finished;
        private bool isRunning;
        private bool isRunningConfirmation;

        private SectionManager sectionManager = null!;

        public Form() : this(new FormOptions())
        {
        }

        public Form(FormOptions options)
        {
            this.options = options;

            sectionsBox = new();
            sectionsIds = [];

            formEvents = new FormEvents();
            formEvents.ReprintEvent += Reprint;
        }

        public FormSection AddSection()
        {
            return AddSection(Guid.NewGuid());
        }

        public FormSection AddSection(object sectionId)
        {
            if (TryAddSection(sectionId, out var section))
                return section;

            throw new ArgumentException("The field id already exists.");
        }

        public bool TryAddSection(object sectionId, [NotNullWhen(true)] out FormSection? section)
        {
            ArgumentNullException.ThrowIfNull(sectionId, nameof(sectionId));

            section = null;

            lock (sectionsIds)
            {
                if (sectionsIds.Contains(sectionId))
                    return false;
            }

            section = new(formEvents, sectionsBox)
            {
                Id = sectionId
            };

            sectionsBox.Add(section);

            lock (sectionsIds)
            {
                sectionsIds.Add(sectionId);
            }

            return true;
        }

        public void Run()
        {
            try
            {
                isRunning = true;
                RunInternal();
            }
            finally
            {
                isRunning = false;
            }
        }

        public void Clear()
        {
            if (isRunning)
                throw new Exception("Can't clear while running.");

            foreach (var section in sectionsBox.GetInstance())
                section.Clear();
        }

        private void RunInternal()
        {
            sectionManager = new(sectionsBox);
            while (true)
            {
                var section = sectionManager.CurrentSection;

                if (!section.AllFinished())
                    section.Run();

                if (options.ConfirmationType == ConfirmationType.None)
                {
                    if (sectionManager.AllFinished)
                    {
                        finished = true; 
                        break;
                    }

                    sectionManager.NextPending();
                    continue;
                }
                else
                {
                    isRunningConfirmation = true;
                    RunConfirmationForms();
                    isRunningConfirmation = false;

                    if (finished)
                        break;
                }
            }

            if (options.PrintAnswersWhenFinish)
            {
                ConsoleWriter.Clear();
                foreach (var section in sectionsBox.GetInstance().Where(e => e.ConditionDelegate()))
                    PrintAnswers(section, false);
            }
        }

        private void RunConfirmationForms()
        {
            if (options.ConfirmationType == ConfirmationType.None)
                return;

            if (!Enum.IsDefined(options.ConfirmationType))
                throw new Exception("Invalid confirmation type");

            List<string> possibilities = GetConfirmationOptions();

            var isEditting = false;
            var confirmationField = new FormItem(formEvents)
                .SingleSelect()
                .Title("Select an option.")
                .Options(possibilities)
                .OnConfirm(value =>
                {
                    if (value == "Confirm form")
                    {
                        finished = true;
                        return;
                    }

                    if (value == "Next pending section")
                    {
                        sectionManager.NextPending();
                        return;
                    }

                    if (value == "Next section")
                    {
                        sectionManager.Next();
                        return;
                    }

                    if (value == "Previous section")
                    {
                        sectionManager.Previous();
                        return;
                    }

                    if (value == "Edit section")
                    {
                        isEditting = true;
                        return;
                    }
                });

            confirmationField.Run();

            if (!isEditting)
                return;

            var fieldSelector = new FormItem(formEvents)
                .MultiSelect<(int Number, FormItem Item)>()
                .Title("Type the number of the field you want to edit")
                .Required(false)
                .Options(() =>
                {
                    var itemsWithNumbers = sectionManager.CurrentSection.FormItemsBox
                        .GetInstance()
                        .Where(e => e.Finished)
                        .Select((e, i) => (i + 1, e));

                    return itemsWithNumbers;
                })
                .ConvertToString(tuple =>
                {
                    return $"{tuple.Number}- {tuple.Item.Field!.GetTitle()}";
                })
                .OnConfirm(tuples =>
                {
                    isEditting = false;
                    if (!tuples.Any())
                    {
                        confirmationField.Reset();
                        return;
                    }

                    foreach (var (_, Item) in tuples)
                        Item.Edit();
                });

            fieldSelector.Run();
        }

        private List<string> GetConfirmationOptions()
        {
            var possibilities = new List<string>();

            if (sectionManager.AllFinished)
                possibilities.Add("Confirm form");
            else
                possibilities.Add("Next pending section");

            if (sectionManager.NextIndex != -1)
                possibilities.Add("Next section");

            if (sectionManager.PreviousIndex != -1)
                possibilities.Add("Previous section");

            possibilities.Add("Edit section");

            return possibilities;
        }

        private void Reprint()
        {
            var section = sectionManager.CurrentSection;
            if (!section.AnyFinished())
            {
                ConsoleWriter.Clear();
                return;
            }

            PrintAnswers(section);
        }

        private void PrintAnswers(FormSection section, bool clearConsole = true)
        {
            StringBuilder stringBuilder = new();

            int itemNumber = 1;
            IEnumerable<FormItem> finishedItems = section.FormItemsBox
                .GetInstance()
                .Where(e => e.Finished && e.ConditionDelegate());

            if (section.NameDelegate is not null)
                stringBuilder.AppendLine(section.NameDelegate());

            foreach (FormItem? item in finishedItems)
            {
                stringBuilder
                    .Append($"{{color:{ConsoleColor.Blue}}}");

                var spacingBuilder = new StringBuilder();

                if (finished || isRunningConfirmation)
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

            if (clearConsole)
                ConsoleWriter.Clear();

            Message.WriteLine(message);
        }
    }
}
