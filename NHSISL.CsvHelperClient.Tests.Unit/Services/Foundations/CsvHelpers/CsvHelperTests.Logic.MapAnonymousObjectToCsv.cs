// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldMapAnonymousObjectToCsvWithNoFieldMappingsAsync(bool withHeader)
        {
            // given
            int count = GetRandomNumber();
            List<Car> randomCars = CreateRandomCars();
            List<object> dynamicCars = CreateAnonymousObjectCars(randomCars);

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

            using StringWriter stringWriter = new StringWriter();
            using CsvWriter csvWriter = new CsvWriter(stringWriter, config);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateStringWriter())
                    .Returns(stringWriter);

            this.csvHelperBrokerMock.Setup(broker =>
                broker.CreateCsvWriter(It.IsAny<StringWriter>(), withHeader))
                    .Returns(csvWriter);

            // when
            string actualCsvFormattedCars = await this.csvHelperService.MapObjectToCsvAsync<dynamic>(
                @object: dynamicCars,
                hasHeaderRecord: withHeader,
                fieldMappings,
                shouldAddTrailingComma: false);

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
