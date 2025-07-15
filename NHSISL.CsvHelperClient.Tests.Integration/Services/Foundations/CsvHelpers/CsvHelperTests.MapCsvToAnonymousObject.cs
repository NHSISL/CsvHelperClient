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
        [Fact(Skip = "Cannot get csvClient to return list of anonymous objects")]
        [Trait("Category", "Integration")]
        public async Task ShouldMapCsvToAnonymousObject()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<object> anonCars = CreateAnonymousObjectCars(randomCars);
            List<object> expectedObjects = anonCars.DeepClone();

            string randomCsvFormattedObjects = GetCsvRepresentationOfAnonymousObject(
                cars: anonCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: false);

            // when
            List<object> retrievedObjects =
                await this.csvClient.MapCsvToObjectAsync<object>(randomCsvFormattedObjects, hasHeaderRecord: true);

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
