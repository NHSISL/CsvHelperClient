// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using CsvHelper;
using System.IO;

namespace NHSISL.CsvHelperClient.Brokers.CsvHelper
{
    internal interface ICsvHelperBroker
    {
        CsvReader CreateCsvReader(StreamReader reader, bool hasHeaderRecord, bool? headerValidated);
        CsvWriter CreateCsvWriter(StreamWriter writer, bool hasHeaderRecord);
    }
}
