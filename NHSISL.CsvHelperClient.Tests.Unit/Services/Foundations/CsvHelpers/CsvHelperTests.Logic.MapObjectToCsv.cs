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
using System.Threading.Tasks;
using Xunit;

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Theory]
        [InlineData(true, false)]
        //[InlineData(false, false)]
        //[InlineData(true, true)]
        //[InlineData(false, true)]
        public async Task ShouldMapObjectToCsvWithoutFieldMappingsAsync(
            bool withHeader,
            bool withTrailingComma)
        {
            // given
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();

            string randomCsvFormattedcars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            //string expectedCsvFormattedCars = randomCsvFormattedcars.DeepClone();

            //List<Car> inputCars = randomCars;
            Dictionary<string, int> fieldMappings = null;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = withHeader,
                NewLine = Environment.NewLine,
                MissingFieldFound = null
            };

            //dynamic myObject = new ExpandoObject();
            //myObject.Rurabajuzo0 = "Vajugonipe";
            //myObject.Rurabajuzo1 = "Vajugonipe";
            //myObject.Rurabajuzo2 = "1111111111";

            var myObject = new List<object>
            {
                new { Rurabajuzo0 = "Vajugonipe", Rurabajuzo1 = "Vajugonipe", Rurabajuzo2 = "1111111111"}
            };

            using StringWriter stringWriter = new StringWriter();
            using CsvWriter csvWriter = new CsvWriter(stringWriter, config);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateStringWriter())
                    .Returns(stringWriter);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader))
                    .Returns(csvWriter);

            // when
            string actualCsvFormattedCars = await this.csvHelperService.MapObjectToCsvAsync(
                @object: myObject,
                hasHeaderRecord: withHeader,
                fieldMappings: fieldMappings,
                shouldAddTrailingComma: withTrailingComma);

            // then
            //actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);
            //actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateStringWriter(),
                    Times.Once);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader),
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
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();

            string randomCsvFormattedcars = GetCsvRepresentationOfCarInReverse(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            string expectedCsvFormattedCars = randomCsvFormattedcars.DeepClone();
            List<Car> inputCars = randomCars;

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

            using StringWriter stringWriter = new StringWriter();
            using CsvWriter csvWriter = new CsvWriter(stringWriter, config);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateStringWriter())
                    .Returns(stringWriter);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader))
                    .Returns(csvWriter);

            // when
            string actualCsvFormattedCars = await this.csvHelperService.MapObjectToCsvAsync(
                @object: inputCars,
                hasHeaderRecord: withHeader,
                fieldMappings,
                shouldAddTrailingComma: withTrailingComma);

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateStringWriter(),
                    Times.Once);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public async Task ShouldMapDynamicToCsvWithNoFieldMappingsAsync(
            bool withHeader,
            bool withTrailingComma)
        {
            // given
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();
            List<dynamic> dynamicCars = CreateDynamicCars(randomCars);

            string randomCsvFormattedcars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: withHeader,
                shouldAddTrailingComma: withTrailingComma);

            string expectedCsvFormattedCars = randomCsvFormattedcars.DeepClone();
            List<Car> inputCars = randomCars;

            Dictionary<string, int> fieldMappings = null;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = withHeader,
                NewLine = Environment.NewLine,
                MissingFieldFound = null
            };

            using StringWriter stringWriter = new StringWriter();
            using CsvWriter csvWriter = new CsvWriter(stringWriter, config);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateStringWriter())
                    .Returns(stringWriter);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader))
                    .Returns(csvWriter);

            // when
            string actualCsvFormattedCars = await this.csvHelperService.MapObjectToCsvAsync(
                @object: dynamicCars,
                hasHeaderRecord: withHeader,
                fieldMappings,
                shouldAddTrailingComma: withTrailingComma);

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateStringWriter(),
                    Times.Once);

            this.csvHelperBrokerMock.Verify(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader),
                    Times.Once());

            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
