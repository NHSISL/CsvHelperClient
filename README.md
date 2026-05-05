# NHSISL - CsvHelperClient

[![The Standard](https://img.shields.io/github/v/release/hassanhabib/The-Standard?filter=v2.10.0&style=default&label=Standard%20Version&color=2ea44f)](https://github.com/hassanhabib/The-Standard)
[![The Standard - COMPLIANT](https://img.shields.io/badge/The_Standard-COMPLIANT-2ea44f)](https://github.com/hassanhabib/The-Standard)
[![NuGet](https://img.shields.io/nuget/v/NHSISL.CsvHelperClient.svg)](https://www.nuget.org/packages/NHSISL.CsvHelperClient)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NHSISL.CsvHelperClient.svg)](https://www.nuget.org/packages/NHSISL.CsvHelperClient)

## Introduction

**CsvHelperClient** is a .NET, [Standard-compliant](https://github.com/hassanhabib/The-Standard) wrapper around the
popular [CsvHelper](https://joshclose.github.io/CsvHelper/) library. It exposes a clean, async-first API for mapping
CSV data to strongly typed .NET objects and vice versa, with built-in support for custom field mappings, optional
headers, and trailing-comma control.

## Features

- ✅ Stream-based — works directly with `Stream`, keeping memory usage low even for large files
- ✅ Fully async — `IAsyncEnumerable<T>` for reading, `ValueTask` for writing
- ✅ Header control — include or omit header rows when reading and writing
- ✅ Custom field mappings — map CSV column indices to object properties by name
- ✅ Trailing comma support — optionally append a trailing comma to each row
- ✅ Cancellation support — all methods accept a `CancellationToken`
- ✅ Structured exceptions — clear validation and dependency exceptions for reliable error handling

## Installation

Install the package from NuGet:

```bash
dotnet add package NHSISL.CsvHelperClient
```

Or search for `NHSISL.CsvHelperClient` in the Visual Studio NuGet Package Manager.

## Quick Start

### 1. Define your model

```csharp
public class Car
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
}
```

### 2. Instantiate the client

`CsvClient` implements `IAsyncDisposable`, so use it inside an `await using` block or manage its lifetime via
dependency injection.

```csharp
await using var csvClient = new CsvClient();
```

---

## Reading CSV Data

### Map CSV to objects (with header row)

When your CSV file contains a header row whose column names match your model's property names, simply pass
`hasHeaderRecord: true`:

```csharp
string csv = """
    Make,Model,Year,Color
    Toyota,Corolla,2022,White
    Ford,Focus,2021,Blue
    """;

byte[] csvBytes = Encoding.UTF8.GetBytes(csv);
using var inputStream = new MemoryStream(csvBytes);

await using var csvClient = new CsvClient();

await foreach (Car car in csvClient.MapCsvToObjectAsync<Car>(
    data: inputStream,
    hasHeaderRecord: true))
{
    Console.WriteLine($"{car.Year} {car.Make} {car.Model} ({car.Color})");
}
```

### Map CSV to objects (no header, with custom field mappings)

When there is no header row, or the column order differs from your model, supply a `fieldMappings` dictionary that
maps each property name to its zero-based column index:

```csharp
// CSV columns: Color(0), Year(1), Model(2), Make(3)
string csv = """
    White,2022,Corolla,Toyota
    Blue,2021,Focus,Ford
    """;

byte[] csvBytes = Encoding.UTF8.GetBytes(csv);
using var inputStream = new MemoryStream(csvBytes);

var fieldMappings = new Dictionary<string, int>
{
    { nameof(Car.Make),  3 },
    { nameof(Car.Model), 2 },
    { nameof(Car.Year),  1 },
    { nameof(Car.Color), 0 }
};

await using var csvClient = new CsvClient();

await foreach (Car car in csvClient.MapCsvToObjectAsync<Car>(
    data: inputStream,
    hasHeaderRecord: false,
    fieldMappings: fieldMappings))
{
    Console.WriteLine($"{car.Year} {car.Make} {car.Model} ({car.Color})");
}
```

---

## Writing CSV Data

### Map objects to CSV (with header row)

```csharp
var cars = new List<Car>
{
    new Car { Make = "Toyota", Model = "Corolla", Year = 2022, Color = "White" },
    new Car { Make = "Ford",   Model = "Focus",   Year = 2021, Color = "Blue"  }
};

using var outputStream = new MemoryStream();

await using var csvClient = new CsvClient();

await csvClient.MapObjectToCsvAsync(
    @object: cars,
    outputStream: outputStream,
    addHeaderRecord: true);

string csvOutput = Encoding.UTF8.GetString(outputStream.ToArray());
Console.WriteLine(csvOutput);
// Make,Model,Year,Color
// Toyota,Corolla,2022,White
// Ford,Focus,2021,Blue
```

### Map objects to CSV (no header)

```csharp
await csvClient.MapObjectToCsvAsync(
    @object: cars,
    outputStream: outputStream,
    addHeaderRecord: false);
```

### Map objects to CSV with custom column order

Use `fieldMappings` to control the column order. Each entry maps a property name to its zero-based output column
index:

```csharp
// Output order: Color(0), Year(1), Model(2), Make(3)
var fieldMappings = new Dictionary<string, int>
{
    { nameof(Car.Make),  3 },
    { nameof(Car.Model), 2 },
    { nameof(Car.Year),  1 },
    { nameof(Car.Color), 0 }
};

await csvClient.MapObjectToCsvAsync(
    @object: cars,
    outputStream: outputStream,
    addHeaderRecord: false,
    fieldMappings: fieldMappings);
```

### Map objects to CSV with a trailing comma

Some downstream systems expect every row to end with a comma. Enable this with `shouldAddTrailingComma`:

```csharp
await csvClient.MapObjectToCsvAsync(
    @object: cars,
    outputStream: outputStream,
    addHeaderRecord: true,
    shouldAddTrailingComma: true);
```

---

## API Reference

### `ICsvClient`

| Method | Description |
|---|---|
| `MapCsvToObjectAsync<T>(Stream, bool, Dictionary<string,int>, bool?, CancellationToken)` | Reads a CSV stream and yields each row as an instance of `T`. |
| `MapObjectToCsvAsync<T>(List<T>, Stream, bool, Dictionary<string,int>, bool?, CancellationToken)` | Serialises a list of `T` objects into the provided output stream as CSV. |

#### `MapCsvToObjectAsync` parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `data` | `Stream` | — | The CSV input stream. Must not be `null`. |
| `hasHeaderRecord` | `bool` | — | `true` if the first row is a header. |
| `fieldMappings` | `Dictionary<string, int>` | `null` | Maps property names to zero-based column indices. When `null`, property names are matched against the header row. |
| `headerValidated` | `bool?` | `true` | Controls CsvHelper's built-in header validation. |
| `cancellationToken` | `CancellationToken` | `default` | Token to cancel the async enumeration. |

#### `MapObjectToCsvAsync` parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `@object` | `List<T>` | — | The list of objects to serialise. Must not be `null`. |
| `outputStream` | `Stream` | — | The stream to write CSV output to. Must not be `null`. |
| `addHeaderRecord` | `bool` | — | `true` to write a header row as the first line. |
| `fieldMappings` | `Dictionary<string, int>` | `null` | Maps property names to zero-based output column indices. |
| `shouldAddTrailingComma` | `bool?` | `false` | `true` to append a trailing comma to every row. |
| `cancellationToken` | `CancellationToken` | `default` | Token to cancel the async write. |

---

## Error Handling

CsvHelperClient surfaces errors through a structured exception hierarchy so you can handle specific failure modes
precisely:

| Exception | When thrown |
|---|---|
| `CsvHelperClientValidationException` | Invalid arguments were supplied (e.g. a `null` stream or `null` object list). |
| `CsvHelperClientDependencyException` | An unexpected error occurred in an underlying dependency. |
| `CsvHelperClientServiceException` | An unexpected error occurred within the service layer. |

Example:

```csharp
try
{
    await foreach (var car in csvClient.MapCsvToObjectAsync<Car>(
        data: inputStream,
        hasHeaderRecord: true))
    {
        Console.WriteLine(car.Make);
    }
}
catch (CsvHelperClientValidationException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (CsvHelperClientDependencyException ex)
{
    Console.WriteLine($"Dependency error: {ex.Message}");
}
```

---

## How to Contribute

If you want to contribute to this project, please review the following documents to gain an understanding of the
patterns and practices used in building this package:

- [The Standard](https://github.com/hassanhabib/The-Standard)
- [C# Coding Standard](https://github.com/hassanhabib/CSharpCodingStandard)
- [The Team Standard](https://github.com/hassanhabib/The-Standard-Team)

To report a bug, please [open a GitHub issue](https://github.com/NHSISL/CsvHelperClient/issues) with as much detail
as possible. If you'd like to contribute, feel free to pick up any open issue and submit a pull request with your
changes.
