// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        //[Fact]
        //public async Task ShouldThrowValidationExceptionOnMapObjectToCsvIfInputsIsInvalidAndLogItAsync()
        //{
        //    // given
        //    List<Car> nullCars = null;
        //    string randomCsvFormattedOptOutData = GetRandomString();
        //    bool withHeaderRecord = true;
        //    Dictionary<string, int> fieldMappings = null;
        //    bool shouldAddTrailingComma = true;

        //    var invalidCsvHelperArgumentsException = new InvalidCsvHelperArgumentsException(
        //        message: "Invalid CSV helper arguments. Please fix the errors and try again.");

        //    invalidCsvHelperArgumentsException.AddData(
        //        key: "Object",
        //        values: "Object is required");

        //    var expectedCsvHelperValidationException =
        //        new CsvHelperValidationException(
        //            message: "CSV helper validation errors occurred, fix the errors and try again.",
        //            innerException: invalidCsvHelperArgumentsException);

        //    // when
        //    ValueTask<string> mapObjectToCsvTask = this.csvHelperService.MapObjectToCsvAsync(
        //        @object: nullCars,
        //        hasHeaderRecord: withHeaderRecord,
        //        fieldMappings,
        //        shouldAddTrailingComma);

        //    CsvHelperValidationException actualCsvHelperValidationException =
        //        await Assert.ThrowsAsync<CsvHelperValidationException>(mapObjectToCsvTask.AsTask);

        //    // then
        //    actualCsvHelperValidationException.Should().BeEquivalentTo(expectedCsvHelperValidationException);
        //    this.csvHelperBrokerMock.VerifyNoOtherCalls();
        //}
    }
}
