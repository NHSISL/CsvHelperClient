using System.Collections.Generic;
using System.Threading.Tasks;

namespace CsvHelperClient.Clients
{
    public interface ICsvHelperClient
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
