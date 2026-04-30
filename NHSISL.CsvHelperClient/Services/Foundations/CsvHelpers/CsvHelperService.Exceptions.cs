// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using Xeptions;

namespace NHSISL.CsvHelperClient.Services.Foundations.CsvHelpers
{
    internal partial class CsvHelperService
    {
        private delegate ValueTask ReturningValueTaskFunction();
        private delegate IAsyncEnumerable<T> ReturningAsyncEnumerableFunction<T>();

        private async IAsyncEnumerable<T> TryCatch<T>(
            ReturningAsyncEnumerableFunction<T> returningAsyncEnumerableFunction,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IAsyncEnumerator<T> enumerator = returningAsyncEnumerableFunction()
                .GetAsyncEnumerator(cancellationToken);

            try
            {
                while (true)
                {
                    bool hasNext;

                    try
                    {
                        hasNext = await enumerator.MoveNextAsync().ConfigureAwait(false);
                    }
                    catch (InvalidCsvHelperArgumentsException invalidCsvHelperArgumentsException)
                    {
                        throw CreateValidationException(invalidCsvHelperArgumentsException);
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        var failedCsvHelperServiceException =
                            new FailedCsvHelperServiceException(
                                message: "Failed CSV helper service error occurred, contact support.",
                                innerException: exception);

                        throw CreateServiceException(failedCsvHelperServiceException);
                    }

                    if (!hasNext)
                    {
                        yield break;
                    }

                    yield return enumerator.Current;
                }
            }
            finally
            {
                await enumerator.DisposeAsync().ConfigureAwait(false);
            }
        }

        private async ValueTask TryCatch(ReturningValueTaskFunction returningValueTaskFunction)
        {
            try
            {
                await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidCsvHelperArgumentsException invalidCsvHelperArgumentsException)
            {
                throw CreateValidationException(invalidCsvHelperArgumentsException);
            }
            catch (InvalidCsvHelperArgumentCombinationException invalidCsvHelperArgumentCombinationException)
            {
                throw CreateValidationException(invalidCsvHelperArgumentCombinationException);
            }
            catch (OperationCanceledException)
            {
                throw;
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
