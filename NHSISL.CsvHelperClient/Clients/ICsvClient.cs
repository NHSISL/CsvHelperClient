// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NHSISL.CsvHelperClient.Clients
{
    public interface ICsvClient : IAsyncDisposable
    {
        IAsyncEnumerable<T> MapCsvToObjectAsync<T>(
            Stream data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? headerValidated = true,
            CancellationToken cancellationToken = default);

        ValueTask MapObjectToCsvAsync<T>(
            List<T> @object,
            Stream outputStream,
            bool addHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? shouldAddTrailingComma = false,
            CancellationToken cancellationToken = default);
    }
}
