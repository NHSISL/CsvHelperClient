// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Xeptions;

namespace NHSISL.CsvHelperClient.Models.Clients.CsvHelpers.Exceptions
{
    public class CsvHelperClientServiceException : Xeption
    {
        public CsvHelperClientServiceException(Xeption innerException)
            : base(message: "CSV Helper Client service error occurred, contact support.",
                  innerException)
        { }
    }
}
