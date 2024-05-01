// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task ShouldThrowServiceExceptionOnMapCsvToObjectIfServiceErrorOccursAndLogItAsync()
        {
            // given
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();
            bool hasHeaderRow = true;
            bool shouldAddTrailingComma = true;

            string inputCsvFormattedOptOutData =
                GetCsvRepresentationOfCar(cars: randomCars, hasHeaderRow, shouldAddTrailingComma);

            Dictionary<string, int> fieldMappings = null;
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
                broker.CreateCsvReader(It.IsAny<StringReader>(), It.IsAny<bool>()))
                    .Throws(serviceException);

            // when
            ValueTask<List<Car>> mapCsvToObjectTask = this.csvHelperService.MapCsvToObjectAsync<Car>(
                data: inputCsvFormattedOptOutData,
                hasHeaderRow,
                fieldMappings);

            CsvHelperServiceException actualCsvHelperServiceException =
                await Assert.ThrowsAsync<CsvHelperServiceException>(mapCsvToObjectTask.AsTask);

            // then
            actualCsvHelperServiceException.Should().BeEquivalentTo(expectedCsvHelperServiceException);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvReader(It.IsAny<StringReader>(), It.IsAny<bool>()),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
