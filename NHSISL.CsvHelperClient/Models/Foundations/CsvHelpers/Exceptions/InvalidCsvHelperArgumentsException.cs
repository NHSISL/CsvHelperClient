﻿// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using Xeptions;

namespace NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions
{
    public class InvalidCsvHelperArgumentsException : Xeption
    {
        public InvalidCsvHelperArgumentsException(string message)
            : base(message)
        { }
    }
}
