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
        public async Task ShouldMapObjectToCsvWithNoFieldMappingsAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> inputCars = randomCars.DeepClone();

            string expectedCsvFormattedCars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: false);

            using MemoryStream outputStream = new MemoryStream();

            // when
            await this.csvClient.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(inputCars),
                outputStream: outputStream,
                addHeaderRecord: true,
                fieldMappings: null,
                shouldAddTrailingComma: false,
                cancellationToken: TestContext.Current.CancellationToken);

            string actualCsvFormattedCars = Encoding.UTF8.GetString(outputStream.ToArray());

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldMapObjectToCsvWithNoHeaderAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> inputCars = randomCars.DeepClone();

            string expectedCsvFormattedCars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: false,
                shouldAddTrailingComma: false);

            using MemoryStream outputStream = new MemoryStream();

            // when
            await this.csvClient.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(inputCars),
                outputStream: outputStream,
                addHeaderRecord: false,
                fieldMappings: null,
                shouldAddTrailingComma: false,
                cancellationToken: TestContext.Current.CancellationToken);

            string actualCsvFormattedCars = Encoding.UTF8.GetString(outputStream.ToArray());

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldMapObjectToCsvWithFieldMappingsAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> inputCars = randomCars.DeepClone();

            string expectedCsvFormattedCars = GetCsvRepresentationOfCarInReverse(
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

            using MemoryStream outputStream = new MemoryStream();

            // when
            await this.csvClient.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(inputCars),
                outputStream: outputStream,
                addHeaderRecord: false,
                fieldMappings: fieldMappings,
                shouldAddTrailingComma: false,
                cancellationToken: TestContext.Current.CancellationToken);

            string actualCsvFormattedCars = Encoding.UTF8.GetString(outputStream.ToArray());

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldMapObjectToCsvWithTrailingCommaAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> inputCars = randomCars.DeepClone();

            string expectedCsvFormattedCars = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: true);

            using MemoryStream outputStream = new MemoryStream();

            // when
            await this.csvClient.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(inputCars),
                outputStream: outputStream,
                addHeaderRecord: true,
                fieldMappings: null,
                shouldAddTrailingComma: true,
                cancellationToken: TestContext.Current.CancellationToken);

            string actualCsvFormattedCars = Encoding.UTF8.GetString(outputStream.ToArray());

            // then
            actualCsvFormattedCars.Should().BeEquivalentTo(expectedCsvFormattedCars);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldThrowValidationExceptionOnMapObjectToCsvIfObjectIsNullAsync()
        {
            // given
            using MemoryStream outputStream = new MemoryStream();

            // when
            ValueTask mapTask = this.csvClient.MapObjectToCsvAsync<Car>(
                @object: (IAsyncEnumerable<Car>)null,
                outputStream: outputStream,
                addHeaderRecord: true,
                cancellationToken: TestContext.Current.CancellationToken);

            // then
            await Assert.ThrowsAsync<
                NHSISL.CsvHelperClient.Models.Clients.CsvHelpers.Exceptions.CsvHelperClientValidationException>(
                    mapTask.AsTask);
        }

        [Fact]
        [Trait("Category", "Acceptance")]
        public async Task ShouldThrowValidationExceptionOnMapObjectToCsvIfOutputStreamIsNullAsync()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            Stream nullOutputStream = null;

            // when
            ValueTask mapTask = this.csvClient.MapObjectToCsvAsync<Car>(
                @object: ToAsyncEnumerable(randomCars),
                outputStream: nullOutputStream,
                addHeaderRecord: true,
                cancellationToken: TestContext.Current.CancellationToken);

            // then
            await Assert.ThrowsAsync<
                NHSISL.CsvHelperClient.Models.Clients.CsvHelpers.Exceptions.CsvHelperClientValidationException>(
                    mapTask.AsTask);
        }
    }
}
