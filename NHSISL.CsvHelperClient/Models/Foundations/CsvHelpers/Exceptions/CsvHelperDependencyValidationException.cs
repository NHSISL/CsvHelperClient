// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Xeptions;

namespace NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions
{
    internal class CsvHelperDependencyValidationException : Xeption
    {
        public CsvHelperDependencyValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
