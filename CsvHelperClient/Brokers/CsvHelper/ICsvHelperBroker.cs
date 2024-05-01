// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.IO;
using CsvHelper;

namespace CsvHelperClient.Brokers.CsvHelper
{
    public interface ICsvHelperBroker
    {
        CsvReader CreateCsvReader(StringReader reader, bool hasHeaderRecord);
        CsvWriter CreateCsvWriter(StringWriter writer, bool hasHeaderRecord);
        StringWriter CreateStringWriter();
    }
}
