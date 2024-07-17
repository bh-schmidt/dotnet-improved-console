using System.Collections;

namespace ImprovedConsole.Forms
{
    public class ConcurrentItemsSync<TItem>
    {
        private readonly object _lock = new();
        private readonly List<TItem> items = [];

        public void Add(TItem item)
        {
            lock (_lock)
            {
                items.Add(item);
            }
        }

        public List<TItem> GetInstance()
        {
            lock (_lock)
            {
                return [.. items];
            }
        }
    }
}
