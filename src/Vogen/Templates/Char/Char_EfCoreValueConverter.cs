﻿
        public class EfCoreValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<VOTYPE, char>
        {
            public EfCoreValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
                : base(
                    vo => vo.Value,
                    value => new VOTYPE(value),
                    mappingHints
                ) { }
            public EfCoreValueConverter()
                : base(
                    vo => vo.Value,
                    value => new VOTYPE(value),
                    null
                ) { }
        }