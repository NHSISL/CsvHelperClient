using FluentAssertions;
using Force.DeepCloner;
using NHSISL.CsvHelperClient.Tests.Integration.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace NHSISL.CsvHelperClient.Tests.Integration.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task ShouldMapCsvToObject()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<Car> expectedObjects = randomCars.DeepClone();

            string randomCsvFormattedObjects = GetCsvRepresentationOfCar(
                cars: randomCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: false);

            // when
            List<Car> retrievedObjects =
                await this.csvClient.MapCsvToObjectAsync<Car>(randomCsvFormattedObjects, hasHeaderRecord: true);

            // then
            retrievedObjects.Should().BeEquivalentTo(expectedObjects);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ShouldMapCsvToDynamicObject()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<dynamic> anonCars = CreateDynamicCars(randomCars);
            List<dynamic> expectedObjects = anonCars.DeepClone();

            string randomCsvFormattedObjects = GetCsvRepresentationOfDynamicObject(
                cars: anonCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: false);

            // when
            List<dynamic> retrievedObjects =
                await this.csvClient.MapCsvToObjectAsync<dynamic>(randomCsvFormattedObjects, hasHeaderRecord: true);

            // then
            retrievedObjects.Should().BeEquivalentTo(expectedObjects);
        }
    }
}
