// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using NHSISL.CsvHelperClient.Brokers.CsvHelper;
using NHSISL.CsvHelperClient.Models.Clients.CsvHelpers.Exceptions;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using NHSISL.CsvHelperClient.Services.Foundations.CsvHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xeptions;

namespace NHSISL.CsvHelperClient.Clients
{
    public sealed class CsvClient : ICsvClient
    {
        private readonly ServiceProvider serviceProvider;
        private readonly ICsvHelperService csvHelperService;

        public CsvClient()
        {
            serviceProvider = RegisterServices();
            csvHelperService = serviceProvider.GetRequiredService<ICsvHelperService>();
        }

        private static ServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<ICsvHelperBroker, CsvHelperBroker>();
            services.AddTransient<ICsvHelperService, CsvHelperService>();

            return services.BuildServiceProvider();
        }

        public async ValueTask DisposeAsync()
        {
            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }

        public async IAsyncEnumerable<T> MapCsvToObjectAsync<T>(
            Stream data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? headerValidated = true,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IAsyncEnumerator<T> enumerator = csvHelperService
                .MapCsvToObjectAsync<T>(
                    data: data,
                    hasHeaderRecord: hasHeaderRecord,
                    fieldMappings: fieldMappings,
                    headerValidated: headerValidated,
                    cancellationToken: cancellationToken)
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
                    catch (CsvHelperValidationException csvHelperValidationException)
                    {
                        var innerException = csvHelperValidationException.InnerException as Xeption;

                        throw new CsvHelperClientValidationException(
                            innerException,
                            innerException?.Data);
                    }
                    catch (CsvHelperDependencyValidationException csvHelperDependencyValidationException)
                    {
                        var innerException = csvHelperDependencyValidationException.InnerException as Xeption;

                        throw new CsvHelperClientValidationException(
                            innerException,
                            innerException?.Data);
                    }
                    catch (CsvHelperDependencyException csvHelperDependencyException)
                    {
                        throw new CsvHelperClientDependencyException(
                            csvHelperDependencyException.InnerException as Xeption);
                    }
                    catch (CsvHelperServiceException csvHelperServiceException)
                    {
                        throw new CsvHelperClientServiceException(
                            csvHelperServiceException.InnerException as Xeption);
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

        public async ValueTask MapObjectToCsvAsync<T>(
            List<T> @object,
            Stream outputStream,
            bool addHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? shouldAddTrailingComma = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await csvHelperService
                    .MapObjectToCsvAsync(
                        @object: @object,
                        outputStream: outputStream,
                        addHeaderRecord: addHeaderRecord,
                        fieldMappings: fieldMappings,
                        shouldAddTrailingComma: shouldAddTrailingComma,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (CsvHelperValidationException csvHelperValidationException)
            {
                var innerException = csvHelperValidationException.InnerException as Xeption;

                throw new CsvHelperClientValidationException(
                    innerException,
                    innerException?.Data);
            }
            catch (CsvHelperDependencyValidationException csvHelperDependencyValidationException)
            {
                var innerException = csvHelperDependencyValidationException.InnerException as Xeption;

                throw new CsvHelperClientValidationException(
                    innerException,
                    innerException?.Data);
            }
            catch (CsvHelperDependencyException csvHelperDependencyException)
            {
                throw new CsvHelperClientDependencyException(
                    csvHelperDependencyException.InnerException as Xeption);
            }
            catch (CsvHelperServiceException csvHelperServiceException)
            {
                throw new CsvHelperClientServiceException(
                    csvHelperServiceException.InnerException as Xeption);
            }
        }
    }
}
