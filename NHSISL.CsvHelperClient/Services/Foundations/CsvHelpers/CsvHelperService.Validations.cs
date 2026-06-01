// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.IO;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using Xeptions;

namespace NHSISL.CsvHelperClient.Services.Foundations.CsvHelpers
{
    internal partial class CsvHelperService
    {
        private static void ValidateMapCsvToObjectArguments(Stream data)
        {
            Validate<InvalidCsvHelperArgumentsException>(
                message: "Invalid CSV helper arguments. Please fix the errors and try again.",
                (Rule: IsInvalid(data), Parameter: "Data"));
        }

        private static void ValidateMapObjectToCsvArguments<T>(T @object)
        {
            Validate<InvalidCsvHelperArgumentsException>(
                message: "Invalid CSV helper arguments. Please fix the errors and try again.",
                (Rule: IsInvalid(@object), Parameter: "Object"));
        }

        private static void ValidateMapObjectToCsvOutputStream(Stream outputStream)
        {
            Validate<InvalidCsvHelperArgumentsException>(
                message: "Invalid CSV helper arguments. Please fix the errors and try again.",
                (Rule: IsInvalid(outputStream), Parameter: "OutputStream"));
        }

        private static void ValidateMapObjectToCsvArgumentCombination(
            bool isPlainObject,
            bool? shouldAddTrailingComma)
        {
            if (isPlainObject && shouldAddTrailingComma == true)
            {
                throw new InvalidCsvHelperArgumentCombinationException(
                    "Invalid CSV helper arguments. Dynamic or anonymous types do not currently " +
                    "have support for trailing commas.");
            }
        }

        private static dynamic IsInvalid(Stream stream) => new
        {
            Condition = stream is null,
            Message = "Stream is required"
        };

        private static dynamic IsInvalid(object @object) => new
        {
            Condition = @object is null,
            Message = "Object is required"
        };

        private static void Validate<T>(
            string message,
            params (dynamic Rule, string Parameter)[] validations)
            where T : Xeption
        {
            var invalidDataException = (T)Activator.CreateInstance(typeof(T), message);

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDataException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDataException.ThrowIfContainsErrors();
        }
    }
}
