// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using NHSISL.CsvHelperClient.Clients;
using NHSISL.CsvHelperClient.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tynamix.ObjectFiller;
using Xunit;

namespace NHSISL.CsvHelperClient.Tests.Integration.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        private readonly CsvClient csvClient;

        public CsvHelperTests()
        {
            this.csvClient = new CsvClient();
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static List<dynamic> CreateDynamicCars(List<Car> cars)
        {
            return cars
                .Select(car =>
                {
                    dynamic item = new ExpandoObject();
                    item.Make = car.Make;
                    item.Model = car.Model;
                    item.Year = car.Year.ToString();
                    item.Color = car.Color;

                    return item;
                })
                .ToList<dynamic>();
        }

        private static List<object> CreateAnonymousObjectCars(List<Car> cars)
        {
            return cars
                .Select(car => new
                {
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Color = car.Color
                })
                .ToList<object>();
        }


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

        public static TheoryData<List<dynamic>> PlainObjectCars()
        {
            List<Car> randomCars = CreateRandomCars();
            List<dynamic> dynamicCars = CreateDynamicCars(randomCars);
            List<dynamic> anonymousObjectCars = CreateAnonymousObjectCars(randomCars);

            return new TheoryData<List<dynamic>>
            {
                dynamicCars,
                anonymousObjectCars
            };
        }

        private string GetCsvRepresentationOfAnonymousObject(
            List<object> cars,
            bool hasHeaderRow,
            bool shouldAddTrailingComma)
        {
            StringBuilder csvBuilder = new StringBuilder();

            if (hasHeaderRow)
            {
                var firstObject = cars[0];

                var headers = firstObject.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Select(p => p.Name);

                string headerLine = string.Join(",", headers);

                csvBuilder.AppendLine(headerLine);
            }

            foreach (var car in cars)
            {
                var values = car.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Select(p => WrapInQuotesIfContainsComma(p.GetValue(car)?.ToString()) ?? string.Empty);

                string line = string.Join(",", values);

                if (shouldAddTrailingComma)
                {
                    line += ",";
                }

                csvBuilder.AppendLine(line);
            }

            return csvBuilder.ToString();
        }

        private string GetCsvRepresentationOfDynamicObject(
            List<dynamic> cars,
            bool hasHeaderRow,
            bool shouldAddTrailingComma)
        {
            StringBuilder csvBuilder = new StringBuilder();

            if (hasHeaderRow)
            {
                csvBuilder.AppendLine("Make,Model,Year,Color");
            }

            foreach (var car in cars)
            {
                string line = $"{WrapInQuotesIfContainsComma(car.Make)}," +
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
    }
}
