// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using Xeptions;

namespace CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions
{
    public class FailedCsvHelperServiceException : Xeption
    {
        public FailedCsvHelperServiceException(string message, Exception innerException)
          : base(message, innerException)
        { }
    }
}
