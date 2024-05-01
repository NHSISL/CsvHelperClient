﻿// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvHelperClient.Services.Foundations.CsvHelpers
{
    public interface ICsvHelperService
    {
        ValueTask<List<T>> MapCsvToObjectAsync<T>(
            string data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings = null);

        ValueTask<string> MapObjectToCsvAsync<T>(
            List<T> @object,
            bool addHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? shouldAddTrailingComma = false);
    }
}
