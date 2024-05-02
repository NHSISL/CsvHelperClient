// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace CsvHelperClient.Brokers.CsvHelper
{
    internal class CsvHelperBroker : ICsvHelperBroker
    {
        public CsvReader CreateCsvReader(StringReader reader, bool hasHeaderRecord)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
                MissingFieldFound = null
            };

            return new CsvReader(reader, config);
        }

        public StringWriter CreateStringWriter()
        {
            return new StringWriter();
        }

        public CsvWriter CreateCsvWriter(StringWriter writer, bool hasHeaderRecord)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
                NewLine = Environment.NewLine,
                MissingFieldFound = null
            };

            return new CsvWriter(writer, config);
        }
    }
}
