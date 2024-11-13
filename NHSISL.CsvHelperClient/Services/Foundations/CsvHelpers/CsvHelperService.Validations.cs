// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using System;
using Xeptions;

namespace CsvHelperClient.Services.Foundations.CsvHelpers
{
    internal partial class CsvHelperService
    {
        private static void ValidateMapCsvToObjectArguments(string data, bool hasHeaderRecord)
        {
            Validate<InvalidCsvHelperArgumentsException>(
                    message: "Invalid CSV helper arguments. Please fix the errors and try again.",
                    (Rule: IsInvalid(data), Parameter: "Data"));
        }

        private static void ValidateMapObjectToCsvArguments<T>(
            T @object, bool hasHeaderRecord)
        {
            Validate<InvalidCsvHelperArgumentsException>(
                    message: "Invalid CSV helper arguments. Please fix the errors and try again.",
                    (Rule: IsInvalid(@object), Parameter: "Object"));
        }

        private static void ValidateMapObjectToCsvArgumentCombination(bool isPlainObject, bool? shouldAddTrailingComma)
        {
            if (isPlainObject == true && shouldAddTrailingComma == true)
            {
                throw new InvalidCsvHelperArgumentCombinationException("Invalid CSV helper arguments. " +
                    "Dynamic or anonymous types do not currently have support for trailing commas.");
            }
        }

        private static dynamic IsInvalid(string text) => new
        {
            Condition = string.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(object @object) => new
        {
            Condition = @object is null,
            Message = "Object is required"
        };

        private static void Validate<T>(string message, params (dynamic Rule, string Parameter)[] validations)
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
