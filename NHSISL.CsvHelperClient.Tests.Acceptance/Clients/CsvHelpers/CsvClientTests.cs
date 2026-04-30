// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHSISL.CsvHelperClient.Clients;
using NHSISL.CsvHelperClient.Tests.Acceptance.Models;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSISL.CsvHelperClient.Tests.Acceptance.Clients.CsvHelpers
{
    public partial class CsvClientTests : IAsyncLifetime
    {
        private readonly CsvClient csvClient;

        public CsvClientTests()
        {
            this.csvClient = new CsvClient();
        }

        public System.Threading.Tasks.Task InitializeAsync() =>
            System.Threading.Tasks.Task.CompletedTask;

        public async System.Threading.Tasks.Task DisposeAsync()
        {
            await this.csvClient.DisposeAsync().ConfigureAwait(false);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static List<Car> CreateRandomCars()
        {
            return CreateCarFiller()
                .Create(count: GetRandomNumber())
                    .ToList();
        }

        private static Filler<Car> CreateCarFiller()
        {
            var filler = new Filler<Car>();
            filler.Setup();

            return filler;
        }

        private string WrapInQuotesIfContainsComma(string value)
        {
            if (value.Contains(","))
            {
                return $"\"{value}\"";
            }

            return value;
        }

        private string GetCsvRepresentationOfCar(
            List<Car> cars,
            bool hasHeaderRow,
            bool shouldAddTrailingComma)
        {
            var csvBuilder = new StringBuilder();

            if (hasHeaderRow)
            {
                csvBuilder.AppendLine("Make,Model,Year,Color");
            }

            foreach (var car in cars)
            {
                string line =
                    $"{WrapInQuotesIfContainsComma(car.Make)}," +
                    $"{WrapInQuotesIfContainsComma(car.Model)}," +
                    $"{WrapInQuotesIfContainsComma(car.Year.ToString())}," +
                    $"{WrapInQuotesIfContainsComma(car.Color)}";

                if (shouldAddTrailingComma)
                {
                    line += ",";
                }

                csvBuilder.AppendLine(line);
            }

            return csvBuilder.ToString();
        }

        private string GetCsvRepresentationOfCarInReverse(
            List<Car> cars,
            bool hasHeaderRow,
            bool shouldAddTrailingComma)
        {
            var csvBuilder = new StringBuilder();

            if (hasHeaderRow)
            {
                csvBuilder.AppendLine("Color,Year,Model,Make");
            }

            foreach (var car in cars)
            {
                string line =
                    $"{WrapInQuotesIfContainsComma(car.Color)}," +
                    $"{WrapInQuotesIfContainsComma(car.Year.ToString())}," +
                    $"{WrapInQuotesIfContainsComma(car.Model)}," +
                    $"{WrapInQuotesIfContainsComma(car.Make)}";

                if (shouldAddTrailingComma)
                {
                    line += ",";
                }

                csvBuilder.AppendLine(line);
            }

            return csvBuilder.ToString();
        }
    }
}
