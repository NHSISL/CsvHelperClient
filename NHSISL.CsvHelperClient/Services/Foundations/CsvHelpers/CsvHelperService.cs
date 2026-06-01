// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NHSISL.CsvHelperClient.Brokers.CsvHelper;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers;
using NHSISL.CsvHelperClient.Services.Foundations.CsvHelpers;

namespace NHSISL.CsvHelperClient.Services.Foundations.CsvHelpers
{
    internal partial class CsvHelperService : ICsvHelperService
    {
        private readonly ICsvHelperBroker csvHelperBroker;

        public CsvHelperService(ICsvHelperBroker csvHelperBroker) =>
            this.csvHelperBroker = csvHelperBroker;

        public IAsyncEnumerable<T> MapCsvToObjectAsync<T>(
            Stream data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? headerValidated = true,
            CancellationToken cancellationToken = default) =>
            TryCatch(
                () => MapCsvToObjectCoreAsync<T>(
                    data: data,
                    hasHeaderRecord: hasHeaderRecord,
                    fieldMappings: fieldMappings,
                    headerValidated: headerValidated),
                cancellationToken: cancellationToken);

        private async IAsyncEnumerable<T> MapCsvToObjectCoreAsync<T>(
            Stream data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings,
            bool? headerValidated,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ValidateMapCsvToObjectArguments(data);
            cancellationToken.ThrowIfCancellationRequested();

            using var reader = new StreamReader(data, leaveOpen: true);
            using var csvReader =
                this.csvHelperBroker.CreateCsvReader(reader, hasHeaderRecord, headerValidated);

            if (fieldMappings != null)
            {
                csvReader.Context.RegisterClassMap(new CustomMap<T>(fieldMappings));
            }

            await foreach (var record in csvReader
                .GetRecordsAsync<T>(cancellationToken)
                .ConfigureAwait(false))
            {
                yield return record;
            }
        }

        public ValueTask MapObjectToCsvAsync<T>(
            IAsyncEnumerable<T> @object,
            Stream outputStream,
            bool addHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? shouldAddTrailingComma = false,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateMapObjectToCsvArguments(@object);
            ValidateMapObjectToCsvOutputStream(outputStream);
            cancellationToken.ThrowIfCancellationRequested();
            bool isPlainObject = typeof(T) == typeof(object);
            ValidateMapObjectToCsvArgumentCombination(isPlainObject, shouldAddTrailingComma);

            await using var streamWriter = new StreamWriter(outputStream, leaveOpen: true);
            await using var csvWriter = this.csvHelperBroker.CreateCsvWriter(streamWriter, addHeaderRecord);

            if (fieldMappings != null)
            {
                csvWriter.Context.RegisterClassMap(new CustomMap<T>(fieldMappings));
            }

            if (addHeaderRecord && !isPlainObject)
            {
                csvWriter.WriteHeader<T>();
                await csvWriter.NextRecordAsync().ConfigureAwait(false);
            }

            bool expandoHeaderWritten = false;
            int rowCount = 0;

            await foreach (var item in @object
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false))
            {
                if (!expandoHeaderWritten && isPlainObject)
                {
                    if (addHeaderRecord)
                    {
                        if (item is IDictionary<string, object> expandoDict)
                        {
                            foreach (var key in expandoDict.Keys)
                            {
                                csvWriter.WriteField(key);
                            }

                            await csvWriter.NextRecordAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            csvWriter.WriteHeader(item.GetType());
                            await csvWriter.NextRecordAsync().ConfigureAwait(false);
                        }
                    }

                    expandoHeaderWritten = true;
                }

                csvWriter.WriteRecord(item);

                if (shouldAddTrailingComma == true)
                {
                    csvWriter.WriteField("");
                }

                await csvWriter.NextRecordAsync().ConfigureAwait(false);

                if (++rowCount % 1000 == 0)
                {
                    await streamWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        });
    }
}
