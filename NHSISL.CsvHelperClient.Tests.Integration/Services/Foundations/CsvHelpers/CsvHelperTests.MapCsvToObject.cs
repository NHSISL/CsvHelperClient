using FluentAssertions;
using Force.DeepCloner;
using NHSISL.CsvHelperClient.Tests.Integration.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

            byte[] csvBytes = Encoding.UTF8.GetBytes(randomCsvFormattedObjects);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            // when
            var retrievedObjects = new List<Car>();

            await foreach (var item in this.csvClient.MapCsvToObjectAsync<Car>(
                inputStream,
                hasHeaderRecord: true))
            {
                retrievedObjects.Add(item);
            }

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

            byte[] csvBytes = Encoding.UTF8.GetBytes(randomCsvFormattedObjects);
            using MemoryStream inputStream = new MemoryStream(csvBytes);

            // when
            var retrievedObjects = new List<dynamic>();

            await foreach (var item in this.csvClient.MapCsvToObjectAsync<dynamic>(
                inputStream,
                hasHeaderRecord: true))
            {
                retrievedObjects.Add(item);
            }

            // then
            retrievedObjects.Should().BeEquivalentTo(expectedObjects);
        }
    }
}
