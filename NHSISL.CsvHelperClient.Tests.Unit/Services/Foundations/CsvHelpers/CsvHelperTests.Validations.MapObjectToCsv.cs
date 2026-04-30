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
        public async Task ShouldThrowValidationExceptionOnMapObjectToCsvIfInputsIsInvalidAndLogItAsync()
        {
            // given
            List<Car> nullCars = null;
            bool withHeaderRecord = true;
            Dictionary<string, int> fieldMappings = null;
            bool shouldAddTrailingComma = true;

            var invalidCsvHelperArgumentsException = new InvalidCsvHelperArgumentsException(
                message: "Invalid CSV helper arguments. Please fix the errors and try again.");

            invalidCsvHelperArgumentsException.AddData(
                key: "Object",
                values: "Object is required");

            var expectedCsvHelperValidationException =
                new CsvHelperValidationException(
                    message: "CSV helper validation errors occurred, fix the errors and try again.",
                    innerException: invalidCsvHelperArgumentsException);

            using MemoryStream outputStream = new MemoryStream();

            // when
            ValueTask mapObjectToCsvTask = this.csvHelperService.MapObjectToCsvAsync(
                @object: nullCars,
                outputStream: outputStream,
                hasHeaderRecord: withHeaderRecord,
                fieldMappings,
                shouldAddTrailingComma);

            CsvHelperValidationException actualCsvHelperValidationException =
                await Assert.ThrowsAsync<CsvHelperValidationException>(mapObjectToCsvTask.AsTask);

            // then
            actualCsvHelperValidationException.Should().BeEquivalentTo(expectedCsvHelperValidationException);
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(PlainObjectCars))]
        public async Task
            ShouldThrowValidationExceptionOnMapCsvToObjectIfPlainObjectAndTrailingCommaInvalidAndLogItAsync(
            List<dynamic> plainObjectCars)
        {
            // given
            bool withHeaderRecord = true;
            Dictionary<string, int> fieldMappings = null;
            bool shouldAddTrailingComma = true;

            var invalidCsvHelperArgumentCombinationException = new InvalidCsvHelperArgumentCombinationException(
                message: "Invalid CSV helper arguments. Dynamic or anonymous types do not currently " +
                    "have support for trailing commas.");

            var expectedCsvHelperValidationException =
                new CsvHelperValidationException(
                    message: "CSV helper validation errors occurred, fix the errors and try again.",
                    innerException: invalidCsvHelperArgumentCombinationException);

            using MemoryStream outputStream = new MemoryStream();

            // when
            ValueTask mapObjectToCsvTask = this.csvHelperService.MapObjectToCsvAsync(
                @object: plainObjectCars,
                outputStream: outputStream,
                hasHeaderRecord: withHeaderRecord,
                fieldMappings,
                shouldAddTrailingComma);

            CsvHelperValidationException actualCsvHelperValidationException =
                await Assert.ThrowsAsync<CsvHelperValidationException>(mapObjectToCsvTask.AsTask);

            // then
            actualCsvHelperValidationException.Should().BeEquivalentTo(expectedCsvHelperValidationException);
            this.csvHelperBrokerMock.VerifyNoOtherCalls();
        }
    }
}
