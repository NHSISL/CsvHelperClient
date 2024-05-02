// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelperClient.Brokers.CsvHelper;
using CsvHelperClient.Models.Foundations.CsvHelpers;

namespace CsvHelperClient.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperService : ICsvHelperService
    {
        private readonly ICsvHelperBroker csvHelperBroker;

        public CsvHelperService(ICsvHelperBroker csvHelperBroker) =>
            this.csvHelperBroker = csvHelperBroker;

        public ValueTask<List<T>> MapCsvToObjectAsync<T>(string data,
            bool hasHeaderRecord,
            Dictionary<string, int>? fieldMappings = null) =>
            TryCatch(async () =>
            {
                ValidateMapCsvToObjectArguments(data, hasHeaderRecord);

                using (var reader = new StringReader(data))
                using (var csvReader = this.csvHelperBroker.CreateCsvReader(reader, hasHeaderRecord))
                {
                    if (fieldMappings != null)
                    {
                        csvReader.Context.RegisterClassMap(new CustomMap<T>(fieldMappings));
                    }

                    var records = csvReader.GetRecords<T>().ToList();

                    return await ValueTask.FromResult(records);
                }
            });

        public ValueTask<string> MapObjectToCsvAsync<T>(
            List<T> @object,
            bool hasHeaderRecord,
            Dictionary<string, int>? fieldMappings = null,
            bool? shouldAddTrailingComma = false) =>
        TryCatch(async () =>
        {
            ValidateMapObjectToCsvArguments(@object, hasHeaderRecord);

            using (var stringWriter = this.csvHelperBroker.CreateStringWriter())
            using (var csvWriter = this.csvHelperBroker.CreateCsvWriter(stringWriter, hasHeaderRecord))
            {

                if (fieldMappings != null)
                {
                    csvWriter.Context.RegisterClassMap(new CustomMap<T>(fieldMappings));
                }

                if (hasHeaderRecord)
                {
                    csvWriter.WriteHeader<T>();
                    csvWriter.NextRecord();
                }

                foreach (var item in @object)
                {
                    csvWriter.WriteRecord(item);

                    if (shouldAddTrailingComma.HasValue && shouldAddTrailingComma.Value == true)
                    {
                        csvWriter.WriteField("");
                    }

                    csvWriter.NextRecord();
                }

                stringWriter.Flush();
                var csv = stringWriter.ToString();

                return await ValueTask.FromResult(csv);
            }
        });
    }
}
