// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using NHSISL.CsvHelperClient.Tests.Unit.Models;
using Xunit;

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task ShouldMapObjectToCsvWithoutFieldMappingsAsync(
            bool withHeader,
            bool withTrailingComma)
        {
            // given
            List<Car> randomCars = CreateRandomCars();

            string randomCsvFormattedcars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            string expectedCsvFormattedCars = randomCsvFormattedcars.DeepClone();
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
            await this.csvHelperService.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(randomCars),
                outputStream: outputStream,
                addHeaderRecord: withHeader,
                fieldMappings: fieldMappings,
                shouldAddTrailingComma: withTrailingComma,
                cancellationToken: TestContext.Current.CancellationToken);

            string actualCsvFormattedCars = Encoding.UTF8.GetString(outputStream.ToArray());

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvWriter(It.IsAny<StreamWriter>(), withHeader),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task ShouldMapObjectToCsvWithFieldMappingsAsync(
            bool withHeader,
            bool withTrailingComma)
        {
            // given
            List<Car> randomCars = CreateRandomCars();

            string randomCsvFormattedcars = GetCsvRepresentationOfCarInReverse(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            string expectedCsvFormattedCars = randomCsvFormattedcars.DeepClone();

            Dictionary<string, int> fieldMappings = new Dictionary<string, int>
            {
                { nameof(Car.Make), 3 },
                { nameof(Car.Model), 2 },
                { nameof(Car.Year), 1 },
                { nameof(Car.Color), 0 }
            };

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
            await this.csvHelperService.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(randomCars),
                outputStream: outputStream,
                addHeaderRecord: withHeader,
                fieldMappings: fieldMappings,
                shouldAddTrailingComma: withTrailingComma,
                cancellationToken: TestContext.Current.CancellationToken);

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
