using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelperClient.Brokers.CsvHelper;
using CsvHelperClient.Models.Clients.CsvHelpers.Exceptions;
using CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using CsvHelperClient.Services.Foundations.CsvHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xeptions;

namespace CsvHelperClient.Clients
{
    public class CsvHelperClient : ICsvHelperClient
    {
        private readonly ICsvHelperService csvHelperService;

        public CsvHelperClient()
        {
            IHost host = RegisterServices();
            csvHelperService = host.Services.GetRequiredService<ICsvHelperService>();
        }

        private static IHost RegisterServices()
        {
            IHostBuilder builder = Host.CreateDefaultBuilder();

            builder.ConfigureServices(configuration =>
            {
                configuration.AddTransient<ICsvHelperBroker, CsvHelperBroker>();
                configuration.AddTransient<ICsvHelperService, CsvHelperService>();
            });

            IHost host = builder.Build();

            return host;
        }

        public async ValueTask<List<T>> MapCsvToObjectAsync<T>(
            string data,
            bool hasHeaderRecord,
            Dictionary<string, int> fieldMappings = null)
        {
            try
            {
                return await csvHelperService.MapCsvToObjectAsync<T>(data, hasHeaderRecord, fieldMappings);
            }
            catch (CsvHelperValidationException csvHelperValidationException)
            {
                throw new CsvHelperClientValidationException(
                    csvHelperValidationException.InnerException as Xeption,
                    csvHelperValidationException.InnerException.Data);
            }
            catch (CsvHelperDependencyValidationException csvHelperDependencyValidationException)
            {
                throw new CsvHelperClientValidationException(
                    csvHelperDependencyValidationException.InnerException as Xeption,
                    csvHelperDependencyValidationException.InnerException.Data);
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

        public async ValueTask<string> MapObjectToCsvAsync<T>(
            List<T> @object,
            bool addHeaderRecord,
            Dictionary<string, int> fieldMappings = null,
            bool? shouldAddTrailingComma = false)
        {
            try
            {
                return await csvHelperService
                    .MapObjectToCsvAsync(@object, addHeaderRecord, fieldMappings, shouldAddTrailingComma);
            }
            catch (CsvHelperValidationException meshOrchestrationValidationException)
            {
                throw new CsvHelperClientValidationException(
                    meshOrchestrationValidationException.InnerException as Xeption,
                    meshOrchestrationValidationException.InnerException.Data);
            }
            catch (CsvHelperDependencyValidationException meshOrchestrationDependencyValidationException)
            {
                throw new CsvHelperClientValidationException(
                    meshOrchestrationDependencyValidationException.InnerException as Xeption,
                    meshOrchestrationDependencyValidationException.InnerException.Data);
            }
            catch (CsvHelperDependencyException meshOrchestrationDependencyException)
            {
                throw new CsvHelperClientDependencyException(
                    meshOrchestrationDependencyException.InnerException as Xeption);
            }
            catch (CsvHelperServiceException csvHelperServiceException)
            {
                throw new CsvHelperClientServiceException(
                    csvHelperServiceException.InnerException as Xeption);
            }
        }
    }
}
