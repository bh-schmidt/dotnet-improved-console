using System.Diagnostics.CodeAnalysis;

namespace ImprovedConsole.Forms
{
    public class FormSection(FormEvents formEvents, ConcurrentItemsSync<FormSection> sectionsBox)
    {
        private readonly HashSet<object> fieldIds = [];
        private readonly ConcurrentItemsSync<FormItem> formItemsBox = new();
        public required object Id { get; set; }
        public Func<bool> ConditionDelegate { get; private set; } = () => true;
        public Func<string>? NameDelegate { get; private set; }

        internal ConcurrentItemsSync<FormItem> FormItemsBox => formItemsBox;

        public FormSection Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));

            NameDelegate = () => name;
            return this;
        }

        public FormSection Name(Func<string> getName)
        {
            NameDelegate = getName ?? throw new ArgumentNullException(nameof(getName));
            return this;
        }

        public FormSection Condition(Func<bool> condition)
        {
            ConditionDelegate = condition;
            return this;
        }

        public void Run()
        {
            while (true)
            {
                var formItems = formItemsBox.GetInstance();
                FormItem? item = formItems.FirstOrDefault(e => !e.Finished && e.ConditionDelegate());

                if (item is null)
                    break;

                var sameAnswer = item.Run();

                if (!sameAnswer)
                {
                    var sections = sectionsBox.GetInstance();
                    var allItems = sections
                        .SelectMany(e => e.formItemsBox.GetInstance(), (Section, Item) => (Section, Item));

                    var dependencies = allItems
                        .Where(e =>
                            e.Item.Finished &&
                            e.Item.Dependencies.Contains(item.Field!));

                    var finishedResets = allItems
                        .Where(e =>
                            e.Item.Finished &&
                            (!e.Section.ConditionDelegate() || !e.Item.ConditionDelegate()));

                    var resetItems = dependencies
                        .Concat(finishedResets)
                        .Distinct()
                        .Select(e => e.Item);

                    ResetItems(resetItems);
                }
            }
        }

        public FormItem Add()
        {
            return Add(Guid.NewGuid());
        }

        public FormItem Add(object fieldId)
        {
            if (TryAdd(fieldId, out var item))
                return item;

            throw new ArgumentException("The field id already exists.");
        }

        public bool TryAdd(object fieldId, [NotNullWhen(true)] out FormItem? item)
        {
            ArgumentNullException.ThrowIfNull(fieldId, nameof(fieldId));

            item = null;

            lock (fieldIds)
            {
                if (fieldIds.Contains(fieldId))
                    return false;
            }

            item = new(formEvents)
            {
                Id = fieldId
            };

            formItemsBox.Add(item);

            lock (fieldIds)
            {
                fieldIds.Add(fieldId);
            }

            return true;
        }

        public void Clear()
        {
            var items = formItemsBox.GetInstance();
            foreach (FormItem formItem in items.Where(e => e.Finished))
                formItem.Reset();
        }

        public bool AnyFinished()
        {
            return formItemsBox.GetInstance().Any(e => e.Finished && e.ConditionDelegate());
        }

        public bool AllFinished()
        {
            return formItemsBox.GetInstance().All(e => e.Finished && e.ConditionDelegate());
        }

        private static void ResetItems(IEnumerable<FormItem> items)
        {
            foreach (FormItem? item in items)
                item.Reset();
        }
    }
}
