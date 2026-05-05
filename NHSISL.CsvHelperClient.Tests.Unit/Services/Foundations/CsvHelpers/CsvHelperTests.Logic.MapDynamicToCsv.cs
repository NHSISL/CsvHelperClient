// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using NHSISL.CsvHelperClient.Tests.Unit.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldMapDynamicToCsvWithNoFieldMappingsAsync(bool withHeader)
        {
            // given
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();
            List<dynamic> dynamicCars = CreateDynamicCars(randomCars);

            string randomCsvFormattedcars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: false);

            string expectedCsvFormattedCars = randomCsvFormattedcars.DeepClone();
            List<Car> inputCars = randomCars;

            Dictionary<string, int> fieldMappings = null;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = withHeader,
                NewLine = Environment.NewLine,
                MissingFieldFound = null
            };

            using MemoryStream outputStream = new MemoryStream();
            using StreamWriter streamWriter = new StreamWriter(outputStream, leaveOpen: true);
            using CsvWriter csvWriter = new CsvWriter(streamWriter, config);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvWriter(It.IsAny<StreamWriter>(), withHeader))
                    .Returns(csvWriter);

            // when
            await this.csvHelperService.MapObjectToCsvAsync<dynamic>(
                @object: dynamicCars,
                outputStream: outputStream,
                addHeaderRecord: withHeader,
                fieldMappings,
                shouldAddTrailingComma: false);

            string actualCsvFormattedCars = Encoding.UTF8.GetString(outputStream.ToArray());

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvWriter(It.IsAny<StreamWriter>(), withHeader),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
