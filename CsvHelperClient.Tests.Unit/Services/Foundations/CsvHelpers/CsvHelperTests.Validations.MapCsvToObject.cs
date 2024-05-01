// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using CsvHelperClient.Tests.Unit.Models;
using FluentAssertions;
using Xunit;

namespace CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnMapCsvToObjectIfInputsIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            string randomCsvFormattedOptOutData = invalidText;
            string inputCsvFormattedOptOutData = randomCsvFormattedOptOutData;
            bool withHeaderRecord = true;

            var invalidCsvHelperArgumentsException = new InvalidCsvHelperArgumentsException(
                message: "Invalid CSV helper arguments. Please fix the errors and try again.");

            invalidCsvHelperArgumentsException.AddData(
                key: "Data",
                values: "Text is required");

            var expectedCsvHelperValidationException =
                new CsvHelperValidationException(
                    message: "CSV helper validation errors occurred, fix the errors and try again.",
                    innerException: invalidCsvHelperArgumentsException);

            // when
            ValueTask<List<Car>> mapCsvToObjectTask = this.csvHelperService.MapCsvToObjectAsync<Car>(
                data: inputCsvFormattedOptOutData,
                hasHeaderRecord: withHeaderRecord);

            CsvHelperValidationException actualCsvHelperValidationException =
                await Assert.ThrowsAsync<CsvHelperValidationException>(mapCsvToObjectTask.AsTask);

            // then
            actualCsvHelperValidationException.Should().BeEquivalentTo(expectedCsvHelperValidationException);
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
