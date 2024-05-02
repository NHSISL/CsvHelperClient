// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using CsvHelper;

namespace NHSISL.CsvHelperClient.Brokers.CsvHelper
{
    internal interface ICsvHelperBroker
    {
        CsvReader CreateCsvReader(StringReader reader, bool hasHeaderRecord);
        CsvWriter CreateCsvWriter(StringWriter writer, bool hasHeaderRecord);
        StringWriter CreateStringWriter();
    }
}
