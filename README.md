# SciNotation.NET

[![NuGet](https://img.shields.io/nuget/v/SciNotation.NET)](https://www.nuget.org/packages/SciNotation.NET)
[![Build Status](https://github.com/datzfain/SciNotation.NET/actions/workflows/publish-nuget.yml/badge.svg)](https://github.com/datzfain/SciNotation.NET/actions)

A simple, zero-dependency .NET library providing extension methods to format numeric values and strings into human-readable scientific notation (mantissa × 10ⁿ).

## Features

* **Universal support**: works with `string`, `double`, `float`, `decimal`, and all integer types.
* **Culture-aware**: honors `IFormatProvider` for decimal separators and number formatting.
* **Special values**: gracefully handles `0`, `NaN`, `Infinity`, `-Infinity`, and subnormal doubles.
* **Configurable precision**: specify the number of decimal places in the mantissa.

## Installation

Install via the .NET CLI:

````bash
dotnet add package SciNotation.NET```

Or via the Package Manager Console:

```powershell
Install-Package SciNotation.NET
````

## Usage

### Formatting a Numeric Value

```csharp
using SciNotation.NET;

class Program
{
    static void Main()
    {
        double value = 12345.6789;
        // Format with 4 decimal places in the mantissa
        string scientific = value.ToScientificNotation(decimals: 4);
        Console.WriteLine(scientific);
        // Output: "1.2346 × 10⁴"
    }
}
```

### Formatting a String Input

```csharp
using SciNotation.NET;

string input = "0.00000123";
string result = input.ToScientificNotation(decimals: 2);
// result == "1.23 × 10⁻⁶"
```

### Culture-Specific Formatting

```csharp
using System.Globalization;
using SciNotation.NET;

var culture = new CultureInfo("de-DE");
string result = (1234.56).ToScientificNotation(decimals: 2, provider: culture);
// result == "1,23 × 10³"
```

## API Reference

| Method                                                                             | Description                                                                              |
| ---------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------- |
| `object.ToScientificNotation(int decimals = 6, IFormatProvider? provider = null)`  | Formats any supported type (`string`, `double`, `float`, `decimal`, integer)             |
| `double.ToScientificNotation(int decimals = 6, IFormatProvider? provider = null)`  | Formats a `double` value into scientific notation using superscript exponent.            |
| `float.ToScientificNotation(int decimals = 6, IFormatProvider? provider = null)`   | Formats a `float` value into scientific notation using superscript exponent.             |
| `decimal.ToScientificNotation(int decimals = 6, IFormatProvider? provider = null)` | Formats a `decimal` value into scientific notation using superscript exponent.           |
| `string.ToScientificNotation(int decimals = 6, IFormatProvider? provider = null)`  | Parses and formats a numeric string into scientific notation using superscript exponent. |

## Building from Source

1. Clone the repository:

   ```bash
   git clone https://github.com/datzfain/SciNotation.NET.git
   cd SciNotation.NET/SciNotation.NET
   ```
2. Restore and build:

   ```bash
   dotnet restore
   dotnet build --configuration Release
   ```

## Running Tests

Navigate to the test project and run:

```bash
cd SciNotation.NET.Tests
dotnet test --configuration Release
```

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch: `git checkout -b feature/YourFeature`.
3. Implement your changes and add tests.
4. Commit your work: `git commit -m "Add feature X"`.
5. Push to your fork and open a Pull Request.

Please follow the existing code style and include unit tests for new functionality.

## License

This project is licensed under the [MIT License](LICENSE).

---

<sub>Maintained by [datzfain](https://github.com/datzfain). Feel free to open issues or PRs!</sub>
