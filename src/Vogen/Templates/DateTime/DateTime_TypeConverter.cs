﻿
        class VOTYPETypeConverter : System.ComponentModel.TypeConverter
        {
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(System.DateTime) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return value switch
                {
                    string stringValue when !string.IsNullOrEmpty(stringValue) && System.DateTime.TryParseExact(stringValue, "O", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind, out var result) => new VOTYPE(result),
                    System.DateTime dateTimeValue => new VOTYPE(dateTimeValue),
                    _ => base.ConvertFrom(context, culture, value),
                };
            }

            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
            {
                return sourceType == typeof(System.DateTime) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
            }

            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType)
            {
                if (value is VOTYPE idValue)
                {
                    if (destinationType == typeof(System.DateTime))
                    {
                        return idValue.Value;
                    }

                    if (destinationType == typeof(string))
                    {
                        return idValue.Value.ToString("O");
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }