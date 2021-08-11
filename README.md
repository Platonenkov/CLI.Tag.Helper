## CLI.Tag.Helper

![Demo](https://github.com/Platonenkov/CLI.Tag.Helper/blob/master/Resources/cli_help.gif)

## Quick start

### Install-Package CLI.Tag.Helper -Version 1.1.0

Open or Create File {PROJECT_NAME}.Tags.json, or input other to the constructor

```json
[
  {
    "Culture": "en-Us",
    "Tags": [
      {
        "Names": [
          "h",
          "help"
        ],
        "Description": "Displays help information about the program",
        "Comments": [
        "Help information"
        ]

      },
      {
        "Names": [
          "l",
          "lang",
          "language"
        ],
        "Description": "Sets the language for programs",
        "Comments": [
        "(--lang en-Us | --lang ru-RU)"
        ]
      }
    ]
  },
  {
    "Culture": "ru-Ru",
    "Tags": [
      {
        "Names": [
          "h",
          "help"
        ],
        "Description": "Выводит справочную информацию по командам командной строки",
        "Comments": [
        "Справочная информация"
        ]

      },
      {
        "Names": [
          "l",
          "lang",
          "language"
        ],
        "Description": "Устанавливает язык для программы",
        "Comments": [
        "(--lang en-Us | --lang ru-RU)"
        ]
      }
    ]
  }

```

`Edit language and tags` what you need.

### Use in you programm:

```C#
await CLITagHelper.PrintSupportedLanguagesAsync();
await CLITagHelper.WriteHelpInfo("en"); //en-Us, en, En - what lang or culture Name you need
```

### [Sample](https://gist.github.com/Platonenkov/a82bb929a7bf0381f24c86c852dab8aa)
