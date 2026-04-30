// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using Moq;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using NHSISL.CsvHelperClient.Tests.Unit.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
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
                broker.CreateCsvReader(It.IsAny<StreamReader>(), It.IsAny<bool>(), It.IsAny<bool>()))
                    .Throws(serviceException);

            byte[] csvBytes = Encoding.UTF8.GetBytes(inputCsvFormattedOptOutData);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            // when
            async Task IterateAsync()
            {
                await foreach (var _ in this.csvHelperService.MapCsvToObjectAsync<Car>(
                    data: inputStream,
                    hasHeaderRow,
                    fieldMappings))
                { }
            }

            CsvHelperServiceException actualCsvHelperServiceException =
                await Assert.ThrowsAsync<CsvHelperServiceException>(IterateAsync);

            // then
            actualCsvHelperServiceException.Should().BeEquivalentTo(expectedCsvHelperServiceException);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvReader(It.IsAny<StreamReader>(), It.IsAny<bool>(), It.IsAny<bool>()),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
