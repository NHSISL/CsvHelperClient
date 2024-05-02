// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Xeptions;

namespace NHSISL.CsvHelperClient.Models.Clients.CsvHelpers.Exceptions
{
    public class CsvHelperClientDependencyException : Xeption
    {
        public CsvHelperClientDependencyException(Xeption innerException)
            : base(message: "CSV Helper Client dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
