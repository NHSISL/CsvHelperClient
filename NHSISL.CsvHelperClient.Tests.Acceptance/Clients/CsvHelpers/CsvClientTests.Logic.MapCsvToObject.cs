// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using NHSISL.CsvHelperClient.Tests.Acceptance.Models;
using Xunit;

namespace NHSISL.CsvHelperClient.Tests.Acceptance.Clients.CsvHelpers
{
    public partial class CsvClientTests
    {
        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldMapCsvToObjectWithNoFieldMappingsAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> expectedCars = randomCars.DeepClone();

            string csvFormattedCars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: false);

            byte[] csvBytes = Encoding.UTF8.GetBytes(csvFormattedCars);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            // when
            var actualCars = new List<Car>();

            await foreach (var car in this.csvClient.MapCsvToObjectAsync<Car>(
                data: inputStream,
                hasHeaderRecord: true))
            {
                actualCars.Add(car);
            }

            // then
            actualCars.Should().BeEquivalentTo(expectedCars);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldMapCsvToObjectWithFieldMappingsAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> expectedCars = randomCars.DeepClone();

            string csvFormattedCars = GetCsvRepresentationOfCarInReverse(
                cars: randomCars,
                hasHeaderRow: false,
                shouldAddTrailingComma: false);

            Dictionary<string, int> fieldMappings = new Dictionary<string, int>
            {
                { nameof(Car.Make), 3 },
                { nameof(Car.Model), 2 },
                { nameof(Car.Year), 1 },
                { nameof(Car.Color), 0 }
            };

            byte[] csvBytes = Encoding.UTF8.GetBytes(csvFormattedCars);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            // when
            var actualCars = new List<Car>();

            await foreach (var car in this.csvClient.MapCsvToObjectAsync<Car>(
                data: inputStream,
                hasHeaderRecord: false,
                fieldMappings: fieldMappings))
            {
                actualCars.Add(car);
            }

            // then
            actualCars.Should().BeEquivalentTo(expectedCars);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldThrowValidationExceptionOnMapCsvToObjectIfDataStreamIsNullAsync()
        {
            // given
            Stream nullStream = null;

            // when
            async Task IterateAsync()
            {
                await foreach (var _ in this.csvClient.MapCsvToObjectAsync<Car>(
                    data: nullStream,
                    hasHeaderRecord: true))
                { }
            }

            // then
            await Assert.ThrowsAsync<
                NHSISL.CsvHelperClient.Models.Clients.CsvHelpers.Exceptions.CsvHelperClientValidationException>(
                    IterateAsync);
        }
    }
}
