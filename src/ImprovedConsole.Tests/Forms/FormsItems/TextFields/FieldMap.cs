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

            yield return new FieldWrapper<string>(events, ["value1", "value2"]);
            yield return new FieldWrapper<bool>(events, [true, false]);
            yield return new FieldWrapper<char>(events, ['a', 'b', 'c']);
            yield return new FieldWrapper<sbyte>(events, [sbyte.MinValue, sbyte.MaxValue]);
            yield return new FieldWrapper<byte>(events, [byte.MinValue, byte.MaxValue]);
            yield return new FieldWrapper<short>(events, [short.MinValue, short.MaxValue]);
            yield return new FieldWrapper<ushort>(events, [ushort.MinValue, ushort.MaxValue]);
            yield return new FieldWrapper<int>(events, [int.MinValue, int.MaxValue]);
            yield return new FieldWrapper<uint>(events, [uint.MinValue, uint.MaxValue]);
            yield return new FieldWrapper<long>(events, [long.MinValue, long.MaxValue]);
            yield return new FieldWrapper<ulong>(events, [ulong.MinValue, ulong.MaxValue]);
            yield return new FieldWrapper<float>(events, [float.MinValue, float.MaxValue]);
            yield return new FieldWrapper<double>(events, [double.MinValue, double.MaxValue]);
            yield return new FieldWrapper<decimal>(events, [decimal.MinValue, decimal.MaxValue]);
            yield return new FieldWrapper<DateTime>(events, [DateTime.Today.AddDays(-1), DateTime.Today, DateTime.Today.AddDays(+1)]);

            // nullable
            yield return new FieldWrapper<bool?>(events, [true, false]);
            yield return new FieldWrapper<char?>(events, ['a', 'b', 'c']);
            yield return new FieldWrapper<sbyte?>(events, [sbyte.MinValue, sbyte.MaxValue]);
            yield return new FieldWrapper<byte?>(events, [byte.MinValue, byte.MaxValue]);
            yield return new FieldWrapper<short?>(events, [short.MinValue, short.MaxValue]);
            yield return new FieldWrapper<ushort?>(events, [ushort.MinValue, ushort.MaxValue]);
            yield return new FieldWrapper<int?>(events, [int.MinValue, int.MaxValue]);
            yield return new FieldWrapper<uint?>(events, [uint.MinValue, uint.MaxValue]);
            yield return new FieldWrapper<long?>(events, [long.MinValue, long.MaxValue]);
            yield return new FieldWrapper<ulong?>(events, [ulong.MinValue, ulong.MaxValue]);
            yield return new FieldWrapper<float?>(events, [float.MinValue, float.MaxValue]);
            yield return new FieldWrapper<double?>(events, [double.MinValue, double.MaxValue]);
            yield return new FieldWrapper<decimal?>(events, [decimal.MinValue, decimal.MaxValue]);
            yield return new FieldWrapper<DateTime?>(events, [DateTime.Today.AddDays(-1), DateTime.Today, DateTime.Today.AddDays(+1)]);

            // other
            yield return new FieldWrapper<Guid>(events, [Guid.NewGuid(), Guid.NewGuid()]);
            yield return new FieldWrapper<Guid?>(events, [Guid.NewGuid(), Guid.NewGuid()]);

        }
    }
}
