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

        public bool IsEditing { get; internal set; }
        public Func<string> GetTitle { get; private set; } = null!;
        public Func<bool> IsRequired { get; private set; } = () => true;
        public Func<string, TFieldType> ConvertFromStringDelegate { get; private set; } = GetConverterWrapped()!;
        public Func<TFieldType, string> ConvertToStringDelegate { get; private set; } = e => e!.ToString()!;
        public IFieldAnswer? Answer { get; protected set; }
        public bool Finished { get; protected set; }

        public TField Title(Func<string> getTitle)
        {
            this.GetTitle = () =>
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

            IsRequired = isRequired;
            return (TField)this;
        }

        public TField Required(bool required)
        {
            return Required(() => required);
        }

        public TField ConvertFromString(Func<string, TFieldType> convert)
        {
            ConverterNotSetException.ThrowIfNull(convert);
            ConvertFromStringDelegate = convert;
            return (TField)this;
        }

        public TField ConvertToString(Func<TFieldType, string> convert)
        {
            ConverterNotSetException.ThrowIfNull(convert);
            ConvertToStringDelegate = convert;
            return (TField)this;
        }

        public TField Converter(IValueConverter<TFieldType> converter)
        {
            return ConvertToString(converter.ConvertToString)
                .ConvertFromString(converter.ConvertFromString);
        }

        public TField ValueConverter<TConverter>()
            where TConverter : IValueConverter<TFieldType>, new()
        {
            var converter = new TConverter();

            return ConvertToString(converter.ConvertToString)
                .ConvertFromString(converter.ConvertFromString);
        }

        public virtual TField ValidateField()
        {
            TitleNotSetException.ThrowIfNull(GetTitle);
            return (TField)this;
        }

        internal bool TryConvertFromString(string value, out TFieldType conversion)
        {
            try
            {
                conversion = ConvertFromStringDelegate(value);
                return true;
            }
            catch (Exception)
            {
                conversion = default!;
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

            if (type.IsEnum)
                return value => Enum.GetName(type, value);

            return null;
        }

        public virtual void SetEdition()
        {
            IsEditing = true;
            Finished = false;
        }

        public abstract IFieldAnswer Run();
        public abstract void Reset();
    }
}
