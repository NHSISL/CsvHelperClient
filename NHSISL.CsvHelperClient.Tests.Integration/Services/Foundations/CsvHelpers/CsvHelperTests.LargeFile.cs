// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NHSISL.CsvHelperClient.Tests.Integration.Models;
using Xunit;

namespace NHSISL.CsvHelperClient.Tests.Integration.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        [Fact]
        [Trait("Category", "Integration")]
        public async Task ShouldRoundTripLargeFileOnDiskAsync()
        {
            // given
            const int rowCount = 100_000;
            string inputFilePath = Path.Combine(Path.GetTempPath(), $"csvhelper_input_{Guid.NewGuid():N}.csv");
            string outputFilePath = Path.Combine(Path.GetTempPath(), $"csvhelper_output_{Guid.NewGuid():N}.csv");

            try
            {
                List<Car> generatedCars = GenerateLargeCarList(rowCount);
                WriteCarsToFile(generatedCars, inputFilePath, hasHeaderRow: true);

                // when — read large input file into objects
                var mappedCars = new List<Car>();

                await using (var inputStream = new FileStream(
                    inputFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 65536,
                    useAsync: true))
                {
                    await foreach (var car in this.csvClient.MapCsvToObjectAsync<Car>(
                        data: inputStream,
                        hasHeaderRecord: true))
                    {
                        mappedCars.Add(car);
                    }
                }

                // when — write mapped objects back out to a second file
                await using (var outputStream = new FileStream(
                    outputFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 65536,
                    useAsync: true))
                {
                    await this.csvClient.MapObjectToCsvAsync<Car>(
                        @object: ToAsyncEnumerable(mappedCars),
                        outputStream: outputStream,
                        addHeaderRecord: true);
                }

                // then — row count is preserved
                mappedCars.Count.Should().Be(rowCount);

                // then — output file content matches input file content exactly
                string inputContent = await File.ReadAllTextAsync(inputFilePath);
                string outputContent = await File.ReadAllTextAsync(outputFilePath);
                outputContent.Should().Be(inputContent);
            }
            finally
            {
                if (File.Exists(inputFilePath))
                {
                    File.Delete(inputFilePath);
                }

                if (File.Exists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }
            }
        }

        private static List<Car> GenerateLargeCarList(int count)
        {
            var filler = CreateCarFiller();
            var cars = new List<Car>(count);

            for (int i = 0; i < count; i++)
            {
                cars.Add(filler.Create());
            }

            return cars;
        }

        private void WriteCarsToFile(List<Car> cars, string filePath, bool hasHeaderRow)
        {
            var csvBuilder = new StringBuilder();

            if (hasHeaderRow)
            {
                csvBuilder.AppendLine("Make,Model,Year,Color");
            }

            foreach (var car in cars)
            {
                csvBuilder.AppendLine(
                    $"{WrapInQuotesIfContainsComma(car.Make)}," +
                    $"{WrapInQuotesIfContainsComma(car.Model)}," +
                    $"{WrapInQuotesIfContainsComma(car.Year.ToString())}," +
                    $"{WrapInQuotesIfContainsComma(car.Color)}");
            }

            File.WriteAllText(filePath, csvBuilder.ToString());
        }
    }
}
