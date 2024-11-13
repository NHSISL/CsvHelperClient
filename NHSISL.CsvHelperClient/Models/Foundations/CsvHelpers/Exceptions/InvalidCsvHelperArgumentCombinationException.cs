using Xeptions;

namespace NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions
{
    public class InvalidCsvHelperArgumentCombinationException : Xeption
    {
        public InvalidCsvHelperArgumentCombinationException(string message)
            : base(message)
        { }
    }
}
