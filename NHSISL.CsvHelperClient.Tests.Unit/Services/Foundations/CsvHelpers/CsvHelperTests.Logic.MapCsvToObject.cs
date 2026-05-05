// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using NHSISL.CsvHelperClient.Tests.Unit.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        public async Task ShouldMapCsvToObjectWithNoFieldMappingsAsync(bool withHeader, bool withTrailingComma)
        {
            // given
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();

            string randomCsvFormattedcars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            string inputCsvFormattedCars = randomCsvFormattedcars;
            List<Car> expectedCars = randomCars.DeepClone();
            bool hasHeaderRecord = withHeader;
            Dictionary<string, int> fieldMappings = null;
            bool headerValidated = true;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
                MissingFieldFound = null,
                HeaderValidated = ConfigurationFunctions.HeaderValidated
            };

            byte[] csvBytes = Encoding.UTF8.GetBytes(inputCsvFormattedCars);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvReader(It.IsAny<StreamReader>(), hasHeaderRecord, headerValidated))
                    .Returns((StreamReader reader, bool header, bool? validated) =>
                        new CsvReader(reader, config));

            // when
            var actualCars = new List<Car>();

            await foreach (var car in this.csvHelperService.MapCsvToObjectAsync<Car>(
                data: inputStream,
                hasHeaderRecord,
                fieldMappings,
                headerValidated))
            {
                actualCars.Add(car);
            }

            // then
            actualCars.Should().BeEquivalentTo(expectedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvReader(It.IsAny<StreamReader>(), hasHeaderRecord, headerValidated),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async Task ShouldMapCsvToObjectWithFieldMappingsAsync(bool withHeader, bool withTrailingComma)
        {
            // given
            List<Car> randomCars = CreateRandomCars();

            string randomCsvFormattedcars = GetCsvRepresentationOfCarInReverse(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            string inputCsvFormattedCars = randomCsvFormattedcars;
            List<Car> expectedCars = randomCars.DeepClone();
            bool hasHeaderRecord = withHeader;
            bool headerValidated = true;

            Dictionary<string, int> fieldMappings = new Dictionary<string, int>
            {
                { nameof(Car.Make), 3 },
                { nameof(Car.Model), 2 },
                { nameof(Car.Year), 1 },
                { nameof(Car.Color), 0 }
            };

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeaderRecord,
                MissingFieldFound = null,
                HeaderValidated = ConfigurationFunctions.HeaderValidated
            };

            byte[] csvBytes = Encoding.UTF8.GetBytes(inputCsvFormattedCars);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvReader(It.IsAny<StreamReader>(), hasHeaderRecord, headerValidated))
                    .Returns((StreamReader reader, bool header, bool? validated) =>
                        new CsvReader(reader, config));

            // when
            var actualOptOuts = new List<Car>();

            await foreach (var car in this.csvHelperService.MapCsvToObjectAsync<Car>(
                data: inputStream,
                hasHeaderRecord: hasHeaderRecord,
                fieldMappings))
            {
                actualOptOuts.Add(car);
            }

            // then
            actualOptOuts.Should().BeEquivalentTo(expectedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvReader(It.IsAny<StreamReader>(), hasHeaderRecord, headerValidated),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
