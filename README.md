# GamaaruLedger

A C# console tool that scrapes and downloads bills that have been passed by the Maldivian Parliament (Majlis).

## What it does

This tool automatically:
1. Fetches the list of bills from the [Maldivian Parliament website](https://majlis.gov.mv)
2. Filters bills with status "Passed at Parliament"
3. Downloads the official passed bill documents (PDF files)
4. Saves them locally with sanitized filenames

## Requirements

- .NET 9.0
- Internet connection

## Usage

```bash
dotnet run
```

The tool will download all passed bills from 2025 to the current directory.

## Dependencies

- [HtmlAgilityPack](https://html-agility-pack.net/) - For HTML parsing

## TODO

- [ ] **Make year configurable**: Currently the year 2025 is hardcoded in the URL (Program.cs:11) and filename (Program.cs:78). Add command-line arguments or configuration to allow users to specify which year(s) to download bills from.
- [ ] Add progress indicators for large downloads
- [ ] Add option to specify output directory
- [ ] Add error handling for network failures and retries
- [ ] Support downloading bills from multiple years in a single run