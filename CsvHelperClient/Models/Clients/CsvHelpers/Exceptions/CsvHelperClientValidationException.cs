// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System.Collections;
using Xeptions;

namespace CsvHelperClient.Models.Clients.CsvHelpers.Exceptions
{
    public class CsvHelperClientValidationException : Xeption
    {
        public CsvHelperClientValidationException(Xeption innerException)
            : base(message: "CV Helper Client validation error(s) occurred, fix the error(s) and try again.",
                  innerException)
        { }

        public CsvHelperClientValidationException(Xeption innerException, IDictionary data)
            : base(message: "CSV Helper Client validation error(s) occurred, fix the error(s) and try again.",
                  innerException,
                  data)
        { }
    }
}
