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
        public async Task ShouldMapCsvToAnonymousObject()
        {
            // given
            List<Car> randomCars = CreateRandomCars();
            List<object> dynamicCars = CreateDynamicObjectCars(randomCars);
            List<object> expectedObjects = dynamicCars.DeepClone();

            string randomCsvFormattedObjects = GetCsvRepresentationOfAnonymousObject(
                cars: dynamicCars,
                hasHeaderRow: true,
                shouldAddTrailingComma: false);

            // when
            List<object> retrievedObjects =
                await this.csvClient.MapCsvToObjectAsync<object>(randomCsvFormattedObjects, hasHeaderRecord: true);

            // then
            retrievedObjects.Should().BeEquivalentTo(expectedObjects);
        }
    }
}
