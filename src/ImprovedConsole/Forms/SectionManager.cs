namespace ImprovedConsole.Forms
{
    public class SectionManager(ConcurrentItemsSync<FormSection> sectionsBox)
    {
        public int PreviousIndex => GetPreviousIndex();
        public int CurrentIndex { get; private set; } = 0;
        public int NextIndex => GetNextIndex();
        public bool AllFinished => sectionsBox.GetInstance()
            .All(e => !e.ConditionDelegate() || e.AllFinished());

        public FormSection CurrentSection { get; private set; } = sectionsBox.GetInstance()[0];

        public void NextPending()
        {
            if (AllFinished)
                return;

            var sections = sectionsBox.GetInstance();
            var pendingSection = sections.First(e => !e.AllFinished());

            CurrentIndex = sections.IndexOf(pendingSection);
            CurrentSection = sectionsBox.GetInstance()[CurrentIndex];
        }

        public void Next()
        {
            if (NextIndex == -1)
                return;

            CurrentIndex = NextIndex;
            CurrentSection = sectionsBox.GetInstance()[CurrentIndex];
        }

        public void Previous()
        {
            if (PreviousIndex == -1)
                return;

            CurrentIndex = PreviousIndex;
            CurrentSection = sectionsBox.GetInstance()[CurrentIndex];
        }

        private int GetPreviousIndex()
        {
            var sections = sectionsBox.GetInstance();

            for (int i = CurrentIndex - 1; i >= 0; i++)
            {
                if (sections[i].ConditionDelegate())
                    return i;
            }

            return -1;
        }

        private int GetNextIndex()
        {
            var sections = sectionsBox.GetInstance();

            for (int i = CurrentIndex + 1; i < sections.Count; i++)
            {
                if (sections[i].ConditionDelegate())
                    return i;
            }

            return -1;
        }
    }
}
