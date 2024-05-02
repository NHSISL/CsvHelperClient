// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Xeptions;

namespace NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions
{
    internal class CsvHelperValidationException : Xeption
    {
        public CsvHelperValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
