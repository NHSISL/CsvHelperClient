// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

// Ignore Spelling: NHSISL

using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace NHSISL.CsvHelperClient.Brokers.CsvHelper
{
    internal class CsvHelperBroker : ICsvHelperBroker
    {
        public CsvReader CreateCsvReader(StreamReader reader, bool hasHeaderRecord, bool? headerValidated = true)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
                MissingFieldFound = null,
                HeaderValidated = headerValidated == true ? ConfigurationFunctions.HeaderValidated : null
            };

            return new CsvReader(reader, config);
        }

        public CsvWriter CreateCsvWriter(StreamWriter writer, bool hasHeaderRecord)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
                NewLine = Environment.NewLine,
                MissingFieldFound = null,
            };

            return new CsvWriter(writer, config);
        }
    }
}
