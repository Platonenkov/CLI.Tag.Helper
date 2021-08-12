## CLI.Tag.Helper

![Demo](https://github.com/Platonenkov/CLI.Tag.Helper/blob/master/Resources/cli_help.gif)

## Quick start

### Install-Package CLI.Tag.Helper -Version 1.2.0.1

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
```C#
        static async Task<int> Main(string[] args)
        {
            await CliTagHelper.CheckForHelpTag_PrintAndCloseAsync(args);

            foreach (var (tag, index) in args.Select((arg, i) => (arg, i)).Where(a => a.arg.StartsWith("-")))
                switch (tag)
                {
                    case "-l":
                    case "--lang":
                    case "--language":
                        {
                            var lang = CliTagHelper.GetTagValueOrError(args, tag, index);
                            CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                            Console.WriteLine($"Culture: {CurrentCulture?.Name}");
                            break;
                        }
                    default:
                        {
                            if (tag.StartsWith('-'))
                            {
                                CliTagHelper.PrintTagInfoAndClose(tag);
                                return -1;
                            }
                            break;
                        }
                }

            return 1;
        }

```
   
<div align="center" >
  <div>
  
  ### Full Extensions
  </div>
</div>
  
|Methods|Description|Описание|
|--|-|-|
|CliTagHelper.`GetTagValueOrError`|Verifying that by the index placed a parameter, but not another tag, and returns its value|Проверка что по индексу лежит есть параметр, но не другой тег, и возвращает его значение |
|CliTagHelper.`GetIndexValueOrError`|Verifies that the tag is followed by its argument, not another tag, and returns its value|Проверка что за тегом следует его аргумент а не другой тег, и возвращает его значение|
|CliTagHelper.`IsItHelpTag`|Whether the tag is help tag|Является ли тег тегом справки (help, h)|
|CliTagHelper.`IsItLanguageTag`|Whether the tag is a language tag|Является ли тег тегом языка|
|CliTagHelper.`GetTagIndex`|Gets the tag index in the argument list|Получает индекс тега в списке аргументов|
|CliTagHelper.`PrintHelpInfo`|Output of available CLI commands to the console|Вывод на консоль доступных команд CLI|
|CliTagHelper.`CheckForHelpTag_PrintAndClose`|Checks whether help should be output to the console (if the -h or --help tag is the first tag)|Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)|
|CliTagHelper.`PrintTagInfo`|Display the tag information on the console|Вывод на консоль информации по тегу|
|CliTagHelper.`PrintTagInfoAndClose`|Displays the tag information and closing the program|Выводит информацию по тегу и останавливает работу программы|
|CliTagHelper.`CheckHelpArgAfterTag_PrintAndClose`|Displays tag information if a -h or --help tag is behind it and closes the application|Выводит информацию по тегу если за ним стоит тег -h или --help и закрывает приложение|
|CliTagHelper.`FindTag`|Searches for tag in available list|Ищет тег в доступных|
|CliTagHelper.`GetLocalizedTags`|Get Localized Tags|Получить локализованные теги|
|CliTagHelper.`GetSupportedLanguages`|Enumerate supported localizations|Перечисление поддерживаемых локализаций|
|CliTagHelper.`PrintSupportedLanguages`|Output of supported tag languages to the console|Вывод на консоль поддерживаемых языков тегов|
|CliTagHelper.`FindLanguageTagValue`|Find language in arguments|Поиск языка в аргументах|

### Use base class
```C#
  var cli = new CliTags(lang, TagFilePath);
  //cli.CurrentCultureTags;
  //cli.ChangeCulture("en")
  //cli.LocalizedTags
  //cli.GetTags()
```
### [Sample](https://gist.github.com/Platonenkov/a82bb929a7bf0381f24c86c852dab8aa)
