using ImprovedConsole.Forms.Exceptions;

namespace ImprovedConsole.Forms.Fields
{
    public abstract class Field<TFieldType, TField> : IField
        where TField : Field<TFieldType, TField>
    {
        private static readonly Dictionary<TypeCode, Func<string, object?>> converters = new()
        {
            {TypeCode.Boolean , value => Convert.ToBoolean(value)},
            {TypeCode.Char , value => Convert.ToChar(value!)},
            {TypeCode.SByte , value => Convert.ToSByte(value)},
            {TypeCode.Byte , value => Convert.ToByte(value)},
            {TypeCode.Int16 , value => Convert.ToInt16(value)},
            {TypeCode.UInt16 , value => Convert.ToUInt16(value)},
            {TypeCode.Int32 , value => Convert.ToInt32(value)},
            {TypeCode.UInt32 , value => Convert.ToUInt32(value)},
            {TypeCode.Int64 , value => Convert.ToInt64(value)},
            {TypeCode.UInt64 , value => Convert.ToUInt64(value)},
            {TypeCode.Single , value => Convert.ToSingle(value)},
            {TypeCode.Double , value => Convert.ToDouble(value)},
            {TypeCode.Decimal , value => Convert.ToDecimal(value)},
            {TypeCode.DateTime , value => Convert.ToDateTime(value)},
            {TypeCode.String , value => value},
        };

        protected Func<bool> isRequired = () => true;
        protected Func<string> getTitle = null!;
        protected Func<string?, TFieldType?> convert = GetConverterWrapped()!;

        public TField Title(Func<string> getTitle)
        {
            this.getTitle = () =>
            {
                var title = getTitle();
                TitleNotSetException.ThrowIfNullOrEmpty(title);
                return title;
            };
            return (TField)this;
        }

        public TField Title(string title)
        {
            return Title(() => title);
        }

        public TField Required(Func<bool> isRequired)
        {
            ArgumentNullException.ThrowIfNull(isRequired);

            this.isRequired = isRequired;
            return (TField)this;
        }

        public TField Required(bool required)
        {
            return Required(() => required);
        }

        public TField ValueConverter(Func<string?, TFieldType?> convert)
        {
            ConverterNotSetException.ThrowIfNull(convert);
            this.convert = convert;
            return (TField)this;
        }

        public TField ValueConverter(IValueConverter<TFieldType> converter)
        {
            return ValueConverter(converter.Convert);
        }

        public TField ValueConverter<TConverter>()
            where TConverter : IValueConverter<TFieldType>, new()
        {
            var converter = new TConverter();
            return ValueConverter(converter);
        }

        public virtual TField ValidateField()
        {
            TitleNotSetException.ThrowIfNull(getTitle);
            ConverterNotSetException.ThrowIfNull(convert);
            return (TField)this;
        }

        protected bool TryConvert(string? value, out TFieldType? conversion)
        {
            try
            {
                conversion = convert(value);
                return true;
            }
            catch (Exception)
            {
                conversion = default;
                return false;
            }
        }

        private static Func<string?, TFieldType?>? GetConverterWrapped()
        {
            var converter = GetConverter(typeof(TFieldType));
            if (converter is null)
                return null;

            return value =>
            {
                if (value is null)
                    return default;

                return (TFieldType?)converter(value);
            };

        }

        private static Func<string, object?>? GetConverter(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType is not null)
                return GetConverter(nullableType);

            var code = Type.GetTypeCode(type);
            if (converters.TryGetValue(code, out var converter))
                return converter;

            if (type == typeof(Guid))
                return value => Guid.Parse(value);

            return null;
        }

        public abstract IFieldAnswer Run();
    }
}
