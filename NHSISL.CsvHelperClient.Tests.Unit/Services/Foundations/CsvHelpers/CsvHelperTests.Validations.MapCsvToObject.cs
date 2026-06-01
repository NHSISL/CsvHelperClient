// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using FluentAssertions;
using NHSISL.CsvHelperClient.Models.Foundations.CsvHelpers.Exceptions;
using NHSISL.CsvHelperClient.Tests.Unit.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace NHSISL.CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnMapCsvToObjectIfInputsIsInvalidAndLogItAsync()
        {
            // given
            Stream nullStream = null;
            bool withHeaderRecord = true;

            var invalidCsvHelperArgumentsException = new InvalidCsvHelperArgumentsException(
                message: "Invalid CSV helper arguments. Please fix the errors and try again.");

            invalidCsvHelperArgumentsException.AddData(
                key: "Data",
                values: "Stream is required");

            var expectedCsvHelperValidationException =
                new CsvHelperValidationException(
                    message: "CSV helper validation errors occurred, fix the errors and try again.",
                    innerException: invalidCsvHelperArgumentsException);

            // when
            async Task IterateAsync()
            {
                await foreach (var _ in this.csvHelperService.MapCsvToObjectAsync<Car>(
                    data: nullStream,
                    hasHeaderRecord: withHeaderRecord,
                    cancellationToken: TestContext.Current.CancellationToken))
                { }
            }

            CsvHelperValidationException actualCsvHelperValidationException =
                await Assert.ThrowsAsync<CsvHelperValidationException>(IterateAsync);

            // then
            actualCsvHelperValidationException.Should().BeEquivalentTo(expectedCsvHelperValidationException);
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
