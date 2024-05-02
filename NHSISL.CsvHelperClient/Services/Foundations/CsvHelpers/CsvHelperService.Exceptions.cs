// ---------------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using Xeptions;

namespace CsvHelperClient.Services.Foundations.CsvHelpers
{
    internal partial class CsvHelperService
    {
        private delegate ValueTask<string> ReturningStringFunction();
        private delegate ValueTask<List<T>> ReturningObjectFunction<T>();

        private async ValueTask<List<T>> TryCatch<T>(ReturningObjectFunction<T> returningObjectFunction)
        {
            try
            {
                return await returningObjectFunction();
            }
            catch (InvalidCsvHelperArgumentsException invalidCsvHelperArgumentsException)
            {
                throw CreateValidationException(invalidCsvHelperArgumentsException);
            }
            catch (Exception exception)
            {
                var failedCsvHelperServiceException =
                    new FailedCsvHelperServiceException(
                        message: "Failed CSV helper service error occurred, contact support.",
                        innerException: exception);

                throw CreateServiceException(failedCsvHelperServiceException);
            }
        }

        private async ValueTask<string> TryCatch(ReturningStringFunction returningStringFunction)
        {
            try
            {
                return await returningStringFunction();
            }
            catch (InvalidCsvHelperArgumentsException invalidCsvHelperArgumentsException)
            {
                throw CreateValidationException(invalidCsvHelperArgumentsException);
            }
            catch (Exception exception)
            {
                var failedCsvHelperServiceException =
                    new FailedCsvHelperServiceException(
                        message: "Failed CSV helper service error occurred, contact support.",
                        innerException: exception);

                throw CreateServiceException(failedCsvHelperServiceException);
            }
        }

        private CsvHelperValidationException CreateValidationException(Xeption exception)
        {
            var csvHelperValidationException = new CsvHelperValidationException(
                message: "CSV helper validation errors occurred, fix the errors and try again.",
                innerException: exception);

            return csvHelperValidationException;
        }

        private CsvHelperServiceException CreateServiceException(Xeption exception)
        {
            var csvHelperServiceException = new CsvHelperServiceException(
                message: "CSV helper service error occurred, contact support.",
                innerException: exception);

            return csvHelperServiceException;
        }
    }
}
