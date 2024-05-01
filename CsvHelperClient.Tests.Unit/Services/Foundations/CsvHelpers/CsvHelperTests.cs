// ---------------------------------------------------------
// Copyright (c) North East London ICB. All rights reserved.
// ---------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CsvHelperClient.Brokers.CsvHelper;
using CsvHelperClient.Services.Foundations.CsvHelpers;
using CsvHelperClient.Tests.Unit.Models;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace CsvHelper.Tests.Unit.Services.Foundations.CsvHelpers
{
    public partial class CsvHelperTests
    {
        private readonly Mock<ICsvHelperBroker> csvHelperBrokerMock;
        private readonly CsvHelperService csvHelperService;

        public CsvHelperTests()
        {
            this.csvHelperBrokerMock = new Mock<ICsvHelperBroker>();

            this.csvHelperService = new CsvHelperService(
                csvHelperBroker: this.csvHelperBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static List<dynamic> CreateDynamicCars(List<Car> cars)
        {
            return cars
                .Select(car => new
                {
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    Color = car.Color
                })
                .ToList<dynamic>();
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

        private string GetCsvRepresentationOfCar(
            List<Car> cars,
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

        private string GetCsvRepresentationOfCarInReverse(
            List<Car> cars,
            bool hasHeaderRow,
            bool shouldAddTrailingComma)
        {
            StringBuilder csvBuilder = new StringBuilder();

            if (hasHeaderRow)
            {
                csvBuilder.AppendLine("Color,Year,Model,Make");
            }

            foreach (var car in cars)
            {
                string line = $"{WrapInQuotesIfContainsComma(car.Color)}," +
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

        private string WrapInQuotesIfContainsComma(string value)
        {
            if (value.Contains(","))
            {
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
