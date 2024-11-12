// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        //[Theory]
        //[InlineData(true, false)]
        //[InlineData(false, false)]
        //[InlineData(true, true)]
        //[InlineData(false, true)]
        //public async Task ShouldMapCsvToObjectWithNoFieldMappingsAsync(bool withHeader, bool withTrailingComma)
        //{
        //    // given
        //    int count = GetRandomNumber();
        //    List<Car> randomCars = CreateRandomCars();

        //    string randomCsvFormattedcars = GetCsvRepresentationOfCar(
        //        cars: randomCars,
        //        hasHeaderRow: withHeader,
        //        shouldAddTrailingComma: withTrailingComma);

        //    string inputCsvFormattedCars = randomCsvFormattedcars;
        //    List<Car> expectedCars = randomCars.DeepClone();
        //    bool hasHeaderRecord = withHeader;
        //    Dictionary<string, int> fieldMappings = null;

        //    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //    {
        //        HasHeaderRecord = hasHeaderRecord,
        //        MissingFieldFound = null
        //    };

        //    using StringReader stringReader = new StringReader(inputCsvFormattedCars);
        //    using CsvReader csvReader = new CsvReader(stringReader, config);

        //    this.csvHelperBrokerMock.Setup(broker =>
        //        broker.CreateCsvReader(It.IsAny<StringReader>(), hasHeaderRecord))
        //            .Returns(csvReader);

        //    // when
        //    List<Car> actualCars = await this.csvHelperService.MapCsvToObjectAsync<Car>(
        //        data: inputCsvFormattedCars,
        //        hasHeaderRecord,
        //        fieldMappings);

        //    // then
        //    actualCars.Should().BeEquivalentTo(expectedCars);

        //    this.csvHelperBrokerMock.Verify(broker =>
        //        broker.CreateCsvReader(It.IsAny<StringReader>(), hasHeaderRecord),
        //            Times.Once());

        //    this.csvHelperBrokerMock.VerifyNoOtherCalls();
        //}

        //[Theory]
        //[InlineData(true, false)]
        //[InlineData(false, false)]
        //[InlineData(true, true)]
        //[InlineData(false, true)]
        //public async Task ShouldMapCsvToObjectWithFieldMappingsAsync(bool withHeader, bool withTrailingComma)
        //{
        //    // given
        //    List<Car> randomCars = CreateRandomCars();

        //    string randomCsvFormattedcars = GetCsvRepresentationOfCarInReverse(
        //        cars: randomCars,
        //        hasHeaderRow: withHeader,
        //        shouldAddTrailingComma: withTrailingComma);

        //    string inputCsvFormattedCars = randomCsvFormattedcars;
        //    List<Car> expectedCars = randomCars.DeepClone();
        //    bool hasHeaderRecord = withHeader;

        //    Dictionary<string, int> fieldMappings = new Dictionary<string, int>
        //    {
        //        { nameof(Car.Make), 3 },
        //        { nameof(Car.Model), 2 },
        //        { nameof(Car.Year), 1 },
        //        { nameof(Car.Color), 0 }
        //    };

        //    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        //    {
        //        HasHeaderRecord = hasHeaderRecord,
        //        MissingFieldFound = null
        //    };

        //    using StringReader stringReader = new StringReader(inputCsvFormattedCars);
        //    using CsvReader csvReader = new CsvReader(stringReader, config);

        //    this.csvHelperBrokerMock.Setup(broker =>
        //        broker.CreateCsvReader(It.IsAny<StringReader>(), hasHeaderRecord))
        //            .Returns(csvReader);

        //    // when
        //    List<Car> actualOptOuts = await this.csvHelperService.MapCsvToObjectAsync<Car>(
        //        data: inputCsvFormattedCars,
        //        hasHeaderRecord: hasHeaderRecord,
        //        fieldMappings);

        //    // then
        //    actualOptOuts.Should().BeEquivalentTo(expectedCars);

        //    this.csvHelperBrokerMock.Verify(broker =>
        //        broker.CreateCsvReader(It.IsAny<StringReader>(), hasHeaderRecord),
        //            Times.Once());

        //    this.csvHelperBrokerMock.VerifyNoOtherCalls();
        //}
    }
}
