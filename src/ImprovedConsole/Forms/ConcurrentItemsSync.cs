using System.Collections;

namespace ImprovedConsole.Forms
{
    public class ConcurrentItemsSync
    {
        private readonly object _lock = new();
        private readonly List<FormItem> items = [];

        public void Add(FormItem item)
        {
            lock (_lock)
            {
                items.Add(item);
            }
        }

        public List<FormItem> GetInstance()
        {
            lock (_lock)
            {
                return [.. items];
            }
        }
    }
}
