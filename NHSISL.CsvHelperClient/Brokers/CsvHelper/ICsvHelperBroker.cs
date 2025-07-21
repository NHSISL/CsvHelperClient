// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using CsvHelper;
using System.IO;

namespace NHSISL.CsvHelperClient.Brokers.CsvHelper
{
    internal interface ICsvHelperBroker
    {
        CsvReader CreateCsvReader(StringReader reader, bool hasHeaderRecord, bool? headerValidated);
        CsvWriter CreateCsvWriter(StringWriter writer, bool hasHeaderRecord);
        StringWriter CreateStringWriter();
    }
}
