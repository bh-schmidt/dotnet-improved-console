using ImprovedConsole.Forms;
using ImprovedConsole.Forms.Fields.TextFields;

namespace ImprovedConsole.Tests.Forms.FormsItems.TextFields
{
    public static class FieldMap
    {
        public static IEnumerable<IFieldWrapper> Fields()
        {
            var events = new FormEvents();
            events.ReprintEvent += () =>
            {
                ConsoleWriter.Clear();
            };

            // primitive
            yield return new FieldWrapper<string>(events, "value");
            yield return new FieldWrapper<bool>(events, true);
            yield return new FieldWrapper<char>(events, 'a');
            yield return new FieldWrapper<sbyte>(events, sbyte.MaxValue);
            yield return new FieldWrapper<byte>(events, byte.MaxValue);
            yield return new FieldWrapper<short>(events, short.MaxValue);
            yield return new FieldWrapper<ushort>(events, ushort.MaxValue);
            yield return new FieldWrapper<int>(events, int.MaxValue);
            yield return new FieldWrapper<uint>(events, uint.MaxValue);
            yield return new FieldWrapper<long>(events, long.MaxValue);
            yield return new FieldWrapper<ulong>(events, ulong.MaxValue);
            yield return new FieldWrapper<float>(events, float.MaxValue);
            yield return new FieldWrapper<double>(events, double.MaxValue);
            yield return new FieldWrapper<decimal>(events, decimal.MaxValue);
            yield return new FieldWrapper<DateTime>(events, DateTime.Today);
            // nullable
            yield return new FieldWrapper<bool?>(events, true);
            yield return new FieldWrapper<char?>(events, 'a');
            yield return new FieldWrapper<sbyte?>(events, sbyte.MaxValue);
            yield return new FieldWrapper<byte?>(events, byte.MaxValue);
            yield return new FieldWrapper<short?>(events, short.MaxValue);
            yield return new FieldWrapper<ushort?>(events, ushort.MaxValue);
            yield return new FieldWrapper<int?>(events, int.MaxValue);
            yield return new FieldWrapper<uint?>(events, uint.MaxValue);
            yield return new FieldWrapper<long?>(events, long.MaxValue);
            yield return new FieldWrapper<ulong?>(events, ulong.MaxValue);
            yield return new FieldWrapper<float?>(events, float.MaxValue);
            yield return new FieldWrapper<double?>(events, double.MaxValue);
            yield return new FieldWrapper<decimal?>(events, decimal.MaxValue);
            yield return new FieldWrapper<DateTime?>(events, DateTime.Today);
            // other
            yield return new FieldWrapper<Guid>(events, Guid.NewGuid());
            yield return new FieldWrapper<Guid?>(events, Guid.NewGuid());
        }
    }
}
