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
                    headerValidated: headerValidated,
                    cancellationToken: cancellationToken),
                cancellationToken: cancellationToken);

        private async IAsyncEnumerable<T> MapCsvToObjectCoreAsync<T>(
            Stream data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings,
            bool? headerValidated,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            ValidateMapCsvToObjectArguments(data, hasHeaderRecord);
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
            List<T> @object,
            Stream outputStream,
            bool hasHeaderRecord,
            Dictionary<string, int>? fieldMappings = null,
            bool? shouldAddTrailingComma = false,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateMapObjectToCsvArguments(@object, hasHeaderRecord);
            ValidateMapObjectToCsvOutputStream(outputStream);
            cancellationToken.ThrowIfCancellationRequested();
            var type = typeof(T);
            bool isPlainObject = type == typeof(object);
            ValidateMapObjectToCsvArgumentCombination(isPlainObject, shouldAddTrailingComma);

            using var streamWriter = new StreamWriter(outputStream, leaveOpen: true);
            using var csvWriter = this.csvHelperBroker.CreateCsvWriter(streamWriter, hasHeaderRecord);

            if (fieldMappings != null)
            {
                csvWriter.Context.RegisterClassMap(new CustomMap<T>(fieldMappings));
            }

            if (isPlainObject)
            {
                await csvWriter.WriteRecordsAsync<T>(@object, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (hasHeaderRecord)
                {
                    csvWriter.WriteHeader<T>();
                    await csvWriter.NextRecordAsync().ConfigureAwait(false);
                }

                foreach (var item in @object)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    csvWriter.WriteRecord(item);

                    if (shouldAddTrailingComma.HasValue && shouldAddTrailingComma.Value == true)
                    {
                        csvWriter.WriteField("");
                    }

                    await csvWriter.NextRecordAsync().ConfigureAwait(false);
                }
            }

            await csvWriter.FlushAsync().ConfigureAwait(false);
            await streamWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        });
    }
}
