using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CLI.Tag.Helper.Exception;

namespace CLI.Tag.Helper
{
    public class CLITagHelper
    {
        #region Static Extensions
        /// <summary>
        /// Вывод на консоль доступных команд CLI
        /// </summary>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task PrintHelpInfoAsync(string lang, string TagFilePath = null)
        {
            var help_tags = await GetLocalizedTagsAsync(lang, TagFilePath);

            if (help_tags?.Tags is null)
            {
                "The program has no reference information".ConsoleRed();
                return;
            }

            foreach (var tag in help_tags.Tags)
                tag.ConsolePrint();
        }
        /// <summary> Вывод на консоль доступных команд CLI </summary>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static void PrintHelpInfo(string lang, string TagFilePath = null)
        {
            var help_tags = GetLocalizedTags(lang, TagFilePath);

            if (help_tags?.Tags is null)
            {
                "The program has no reference information".ConsoleRed();
                return;
            }

            foreach (var tag in help_tags.Tags)
                tag.ConsolePrint();
        }

        /// <summary> Вывод на консоль информации по тегу </summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task PringTagInfoAsync(string tag, string lang, string TagFilePath = null)
        {
            var help_tags = await GetLocalizedTagsAsync(lang, TagFilePath);

            if (help_tags?.FindTag(tag) is not { } find)
            {
                $"UNKNOWN tag: {tag}".ConsoleRed();
                return;
            }

            find.ConsolePrint();
        }
        /// <summary> Вывод на консоль информации по тегу </summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static void PrintTagInfo(string tag, string lang, string TagFilePath = null)
        {
            var help_tags = GetLocalizedTags(lang, TagFilePath);

            if (help_tags?.FindTag(tag) is not { } find)
            {
                "UNKNOWN tag".ConsoleRed();
                return;
            }

            find.ConsolePrint();
        }
        /// <summary> Ищет тег в доступных</summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task<Tag> FindTagAsync(string tag, string lang, string TagFilePath = null)
        {
            var help_tags = await GetLocalizedTagsAsync(lang, TagFilePath);
            return help_tags?.FindTag(tag);
        }
        /// <summary> Ищет тег в доступных</summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static Tag FindTag(string tag, string lang, string TagFilePath = null)
        {
            var help_tags = GetLocalizedTags(lang, TagFilePath);
            return help_tags?.FindTag(tag);
        }

        #region Get

        /// <summary> Получить список локализованных тегов </summary>
        /// <param name="lang">культура языка</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task<LocalizedTags> GetLocalizedTagsAsync(string lang, string TagFilePath = null) =>
            await Task.Run(() => GetLocalizedTags(lang, TagFilePath)).ConfigureAwait(false);

        /// <summary> Получить список локализованных тегов </summary>
        /// <param name="lang">культура языка</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static LocalizedTags GetLocalizedTags(string lang, string TagFilePath)
        {
            var cli = new CLITagHelper(lang, TagFilePath);
            return cli.CurrentCultureTags;
        }
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>

        #endregion

        #region Поддержка языков

        public static async Task<IEnumerable<string>> GetSupportedLanguagesAsync(string TagFilePath = null) =>
            await Task.Run(() => GetSupportedLanguages(TagFilePath)).ConfigureAwait(false);
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static IEnumerable<string> GetSupportedLanguages(string TagFilePath = null)
        {
            var cli = new CLITagHelper((CultureInfo)null, TagFilePath);
            return cli.SupportedCultures;
        }
        /// <summary> Вывод на консоль поддерживаемых языков тегов </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static async Task PrintSupportedLanguagesAsync(string TagFilePath = null)
        {
            var languages = (await GetSupportedLanguagesAsync(TagFilePath)).ToArray();
            if (languages.Length > 0)
                $"Supported languages: {string.Join(", ", languages)}".ConsoleYellow();
        }
        /// <summary> Вывод на консоль поддерживаемых языков тегов </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static void PrintSupportedLanguages(string TagFilePath = null)
        {
            var languages = GetSupportedLanguages(TagFilePath).ToArray();
            if (languages.Length > 0)
                $"Supported languages: {string.Join(", ", languages)}".ConsoleGreen();
        }

        #endregion

        #endregion

        #region Конструкторы

        /// <summary> конструктор </summary>
        /// <param name="culture">культура для поиска тегов</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public CLITagHelper(CultureInfo culture, string TagFilePath = null) => Initialize(culture?.Name, TagFilePath);
        /// <summary> конструктор </summary>
        /// <param name="locale">название языка для поиска тегов</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public CLITagHelper(string locale, string TagFilePath = null) => Initialize(locale, TagFilePath);

        private void Initialize(string locale, string TagFilePath)
        {
            LocalizedTags = GetTags(TagFilePath);
            CurrentCulture = locale;
        }

        #endregion
        /// <summary> Перечисление локализованных тегов CLI </summary>
        public IReadOnlyCollection<LocalizedTags> LocalizedTags { get; private set; }
        /// <summary> Текущий язык или культура </summary>
        public string CurrentCulture { get; private set; }
        /// <summary> текущая локализация тегов </summary>
        public LocalizedTags CurrentCultureTags
        {
            get
            {
                var tags = LocalizedTags;
                var current_culture = CurrentCulture?.ToUpper();
                if (string.IsNullOrWhiteSpace(current_culture))
                    return tags.FirstOrDefault();

                var current = tags.FirstOrDefault(
                    t => t.Culture?.ToUpper() is { Length: > 0 } current_tag
                         &&
                         (current_tag == current_culture ||
                          current_tag.Split('-')[0] == current_culture ||
                          current_tag == current_culture.Split('-')[0]));
                return current ?? tags.FirstOrDefault();
            }
        }

        /// <summary> Устанавливает язык для получения локализованных тегов </summary>
        /// <param name="culture"></param>
        public void SetCulture(CultureInfo culture) => CurrentCulture = culture.Name;

        #region Get Tags

        /// <summary> Путь к файлу с тегами </summary>
        private static string TagHelpFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{AppDomain.CurrentDomain.FriendlyName}.Tags.json");
        /// <summary> Получает перечисление известных тегов с описанием из файла </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns>перечисление тегов с описанием</returns>
        public IReadOnlyCollection<LocalizedTags> GetTags(string TagFilePath = null)
        {
            var file_path = TagFilePath ?? TagHelpFilePath;
            if (!File.Exists(file_path)) return new List<LocalizedTags>();
            var json_file = File.ReadAllText(file_path);
            return JsonSerializer.Deserialize<IReadOnlyCollection<LocalizedTags>>(json_file);
        }

        #endregion

        /// <summary> Перечисление поддерживаемых локализаций </summary>
        public IEnumerable<string> SupportedCultures => LocalizedTags.Select(t => t.Culture);


        #region Supports
        /// <summary> Проверка что за тегом следует его аргумент а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="index">текущий индекс тега</param>
        /// <returns>заданный тегом параметр</returns>
        public static string CheckValueIndexError(string[] args, int index)
        {
            if (index == args.Length - 1 || args[index + 1].StartsWith("-"))
                throw new CLIHelpConfigurationException($"Key {args[index]} defined, but parameter is not defined.");
            return args[index + 1];
        }
        /// <summary> Проверка что за тегом следует его аргумент а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="tag">текущий тег</param>
        /// <returns>заданный тегом параметр</returns>
        public static string CheckTagValueError(string[] args, string tag)
        {
            var index = GetTagIndex(args, tag);
            if (index == -1)
                throw new CLIHelpConfigurationException($"Key {tag} is not defined.");
            PrintTagHelpInfoAndClose(args, tag, index);

            return CheckValueIndexError(args, index);
        }
        /// <summary>
        /// Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <returns></returns>
        public static async Task CheckForHelpTagAndPrintAsync(string[] args)
        {
            if (args.Length > 0 && args[0] is "-h" or "--help")
            {
                var lang = FindLanguageTagValue(args);
                await PrintSupportedLanguagesAsync();
                await PrintHelpInfoAsync(lang); //en-Us, en, En - what lang or culture name you need
                Environment.Exit(1);
            }
        }
        /// <summary>
        /// Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <returns></returns>
        public static void CheckForHelpTagAndPrint(string[] args)
        {
            if (args.Length > 0 && args[0] is "-h" or "--help")
            {
                var lang = FindLanguageTagValue(args);
                PrintSupportedLanguages();
                PrintHelpInfo(lang); //en-Us, en, En - what lang or culture name you need
            }
        }
        /// <summary>
        /// Является ли тег идентификатором языка
        /// </summary>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool IsItLanguageTag(string tag) => tag.Trim('-') is "l" or "lang" or "language";
        /// <summary>
        /// Является ли тег идентификатором языка
        /// </summary>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool IsItLanguageTag(char tag) => IsItLanguageTag($"{tag}");
        /// <summary>
        /// Является ли тег help
        /// </summary>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool IsItHelpTag(string tag) => tag.Trim('-') is "h" or "help";
        /// <summary>
        /// Является ли тег help
        /// </summary>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool IsItHelpTag(char tag) => IsItHelpTag($"{tag}");
        /// <summary> Выводит информацию по тегу и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag_index">индекс аргумента</param>
        /// <param name="tag">тег</param>
        public static void PrintTagHelpInfoAndClose(string[] args, string tag, int tag_index)
        {
            var count = args.Length;
            if (!string.IsNullOrWhiteSpace(tag) && tag_index + 1 < count)
            {
                var trimmed_tag = tag.Trim('-');
                var arg = args[tag_index + 1];
                if (IsItHelpTag(arg))
                {

                    var lang = IsItLanguageTag(trimmed_tag) ? null : FindLanguageTagValue(args);

                    var help = new CLITagHelper(lang).CurrentCultureTags?.Tags?.FirstOrDefault(t => t.Names.Contains(trimmed_tag.Trim('-')));
                    help?.ConsolePrint();
                    Environment.Exit(1);
                }
                else if (IsItLanguageTag(trimmed_tag) && tag_index + 2 < count && IsItHelpTag(args[tag_index+2]))
                {
                    PrintHelpAndClose(trimmed_tag, arg);
                }
            }
        }
        /// <summary>
        /// Выводит информацию по тегу и останавливает работу программы
        /// </summary>
        /// <param name="tag">тег</param>
        /// <param name="lang">язык</param>
        public static void PrintHelpAndClose(string tag, string lang)
        {
            PrintTagInfo(tag, lang);
            Environment.Exit(1);
        }
        /// <summary>
        /// Получает индекс тега в списке аргументов
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <returns>индекс тега или -1 если отсутствует в списке</returns>
        public static int GetTagIndex(string[] args, string tag) => args.FirstIndexOf(tag.StartsWith("-") ? tag : Tag.GetFullTag(tag));
        /// <summary>
        /// Получает индекс тега в списке аргументов
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <returns>индекс тега или -1 если отсутствует в списке</returns>
        public static int GetTagIndex(string[] args, char tag) => args.FirstIndexOf(Tag.GetFullTag($"{tag}"));

        /// <summary> Выводит информацию по тегу если за ним стоит тег -h или --help и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        public static void PrintTagHelpInfoAndClose(string[] args, char tag) => PrintTagHelpInfoAndClose(args, $"{tag}");
        /// <summary> Выводит информацию по тегу если за ним стоит тег -h или --help и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        public static void PrintTagHelpInfoAndClose(string[] args, string tag)
        {
            var index = args.FirstIndexOf(tag.StartsWith("-") ? tag : Tag.GetFullTag(tag));
            if (index == -1 || index + 1 >= args.Length)
                return;

            if (string.IsNullOrWhiteSpace(tag) || args[index + 1] is not ("-h" or "--help")) return;
            var lang = FindLanguageTagValue(args);

            var help = new CLITagHelper(lang).CurrentCultureTags?.Tags?.FirstOrDefault(t => t.Names.Contains(tag));
            help?.ConsolePrint();
            Environment.Exit(1);
        }
        /// <summary> Поиск языка в аргументах </summary>
        /// <param name="args">аргументы</param>
        /// <returns>язык</returns>
        public static string FindLanguageTagValue(string[] args)
        {
            var lang_arg = args.FirstOrDefault(a => a is "-l" or "--lang" or "--language");
            if (lang_arg is not null && args.FirstIndexOf(lang_arg) is var lang_index)
            {
                PrintTagHelpInfoAndClose(args, lang_arg, lang_index);

                return GetLanguageValue(args, lang_index);
            }
            return null;
        }
        /// <summary>
        /// устанавливает язык контейнера при наличии в аргументах тегов определения языка
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="lang_index">индекс языка</param>
        /// <returns>значение тега языкам</returns>
        public static string GetLanguageValue(string[] args, int lang_index)
        {
            CheckValueIndexError(args, lang_index);
            return args[lang_index + 1];
        }

        #endregion
    }
}