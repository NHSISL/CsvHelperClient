// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using CsvHelperClient.Tests.Unit.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnMapObjectToCsvIfServiceErrorOccursAndLogItAsync()
        {
            // given
            int count = GetRandomNumber();
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
                broker.CreateStringWriter())
                    .Throws(serviceException);

            // when
            ValueTask<string> mapCsvToObjectTask = this.csvHelperService.MapObjectToCsvAsync<Car>(
                @object: randomCars,
                hasHeaderRecord: withHeaderRecord,
                fieldMappings,
                shouldAddTrailingComma);

            CsvHelperServiceException actualCsvHelperServiceException =
                await Assert.ThrowsAsync<CsvHelperServiceException>(mapCsvToObjectTask.AsTask);

            // then
            actualCsvHelperServiceException.Should().BeEquivalentTo(expectedCsvHelperServiceException);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateStringWriter(),
                        Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
