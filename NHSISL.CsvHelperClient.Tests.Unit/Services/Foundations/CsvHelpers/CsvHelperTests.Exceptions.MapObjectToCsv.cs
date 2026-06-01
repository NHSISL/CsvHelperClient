// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using NHSISL.CsvHelperClient.Tests.Unit.Models;
using Xunit;

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionOnMapObjectToCsvIfCanceledAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            bool withHeaderRecord = true;
            Dictionary<string, int> fieldMappings = null;
            bool shouldAddTrailingComma = false;
            using MemoryStream outputStream = new MemoryStream();
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // when
            ValueTask mapObjectToCsvTask = this.csvHelperService.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(randomCars),
                outputStream: outputStream,
                addHeaderRecord: withHeaderRecord,
                fieldMappings: fieldMappings,
                shouldAddTrailingComma: shouldAddTrailingComma,
                cancellationToken: cancellationTokenSource.Token);

            // then
            await Assert.ThrowsAsync<OperationCanceledException>(mapObjectToCsvTask.AsTask);
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnMapObjectToCsvIfServiceErrorOccursAndLogItAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            bool withHeaderRecord = true;
            Dictionary<string, int> fieldMappings = null;
            bool shouldAddTrailingComma = true;
            var serviceException = new Exception();

            var failedCsvHelperServiceException =
                new FailedCsvHelperServiceException(
                    message: "Failed CSV helper service error occurred, contact support.",
                    innerException: serviceException);

            var expectedCsvHelperServiceException =
                new CsvHelperServiceException(
                    message: "CSV helper service error occurred, contact support.",
                    innerException: failedCsvHelperServiceException);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvWriter(It.IsAny<StreamWriter>(), It.IsAny<bool>()))
                    .Throws(serviceException);

            using MemoryStream outputStream = new MemoryStream();

            // when
            ValueTask mapObjectToCsvTask = this.csvHelperService.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(randomCars),
                outputStream: outputStream,
                addHeaderRecord: withHeaderRecord,
                fieldMappings: fieldMappings,
                shouldAddTrailingComma: shouldAddTrailingComma,
                cancellationToken: TestContext.Current.CancellationToken);

            CsvHelperServiceException actualCsvHelperServiceException =
                await Assert.ThrowsAsync<CsvHelperServiceException>(mapObjectToCsvTask.AsTask);

            // then
            actualCsvHelperServiceException.Should().BeEquivalentTo(expectedCsvHelperServiceException);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvWriter(It.IsAny<StreamWriter>(), It.IsAny<bool>()),
                        Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
