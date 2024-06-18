using System.Text;

namespace ImprovedConsole.Forms.Fields
{
    public interface IFieldAnswer : IEquatable<IFieldAnswer>
    {
        public IField Field { get; }
        public StringBuilder GetFormattedAnswer(int leftSpacing, FormOptions options);
    }
}
