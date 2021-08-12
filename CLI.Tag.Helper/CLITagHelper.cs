using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CLI.Tag.Helper.Exception;

namespace CLI.Tag.Helper
{
    public static class CLITagHelper
    {
        #region Checks

        /// <summary>
        /// Проверка что после тега есть параметр
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <param name="index">индекс тега</param>
        /// <returns></returns>
        public static bool TagHasValue(string[] args, int index) =>
            !(index == args.Length - 1 || index < 0 || args[index + 1].StartsWith("-"));
        /// <summary>
        /// Проверка что после тега есть параметр
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool TagHasValue(string[] args, string tag) => TagHasValue(args, GetTagIndex(args, tag));
        /// <summary>
        /// Проверка что после тега есть несколько параметров
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <param name="index">индекс тега</param>
        /// <returns></returns>
        public static bool TagHasMultipleValue(string[] args, int index) =>
            TagHasValue(args, index) && TagHasValue(args, index + 1);


        #endregion

        #region Arguments value
        /// <summary>
        /// Возвращает перечисление аргументов и их позиции индексов в списке аргументов
        /// </summary>
        /// <param name="args">список аргументов</param>
        /// <returns>tag и index</returns>
        public static IEnumerable<(string tag, int index)> GetArguments(string[] args)
            => args?.Select((arg, i) => (arg, i)).Where(a => a.arg.StartsWith("-"));
        /// <summary>
        /// Возвращает перечисление аргументов и их позиции индексов в списке аргументов
        /// </summary>
        /// <param name="args">список аргументов</param>
        /// <returns>tag, index, parameters</returns>
        public static IEnumerable<(string tag, int index, IEnumerable<string> parameters)> GetArgumentsWithValues(string[] args)
            => GetArguments(args)?.Select(a => (a.tag, a.index, GetTagMultipleValueOrError(args, a.tag, a.index, false)));
        /// <summary>
        /// Возвращает перечисление аргументов и их позиции индексов в списке аргументов
        /// </summary>
        /// <param name="args">список аргументов</param>
        /// <returns>tag, index, parameters</returns>
        public static IEnumerable<(string tag, int index, string parameters)> GetArgumentsWithOneStringValues(string[] args)
            => GetArgumentsWithValues(args)
               .Select(v => (v.tag, v.index, v.parameters?.ToArray() is { Length: > 0 } values ? string.Join(", ", values) : null));

        /// <summary>
        /// Проверка что после тега есть несколько параметров
        /// </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool TagHasMultipleValue(string[] args, string tag) => TagHasMultipleValue(args, GetTagIndex(args, tag));

        /// <summary> Проверка что по индексу лежит есть параметр, но не другой тег, и возвращает его значение </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="index">текущий индекс тега</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега (в случае отсутствия - будет ошибка)</param>
        /// <param name="ErrorIfMultiple">Воспроизвести ошибку, если у тега несколько параметров</param>
        /// <returns>заданный тегом параметр</returns>
        public static string GetIndexValueOrError(string[] args, int index, bool ErrorIfNoValue = true, bool ErrorIfMultiple = true)
        {
            if (!TagHasValue(args, index))
                return ErrorIfNoValue ? throw new CLIHelpConfigurationException($"Key {args[index]} defined, but parameter is not defined.") : null;
            if (ErrorIfMultiple)
                CheckMultipleValueErrorForSingleValueTag(args, args[index], index);

            return args[index + 1];
        }

        /// <summary> Проверка что за тегом следует его аргумент а не другой тег, и возвращает его значение </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="tag">текущий тег</param>
        /// <param name="tag_index">индекс текущего тега</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега</param>
        /// <param name="ErrorIfMultiple">Воспроизвести ошибку, если у тега несколько параметров</param>
        /// <returns>заданный тегом параметр</returns>
        public static string GetTagValueOrError(string[] args, string tag, int tag_index, bool ErrorIfNoValue = true, bool ErrorIfMultiple = true)
        {
            if (tag_index < 0 || tag_index >= args.Length)
                return ErrorIfNoValue ? throw new CLIHelpConfigurationException($"Key {tag} is not defined.") : null;

            CheckHelpArgAfterTag_PrintAndClose(args, tag, tag_index);
            return GetIndexValueOrError(args, tag_index, ErrorIfNoValue, ErrorIfMultiple);
        }

        private static void CheckMultipleValueErrorForSingleValueTag(string[] args, string tag, int tag_index)
        {
            var next = tag_index + 2;
            if (next < args.Length && !args[next].StartsWith("-"))
                throw new CLIHelpConfigurationException(
                    $"Key {tag} defined with multiple parameters, but this tag must defined with single."
                    + $"\nCheck parameter {args[next]}");

        }
        /// <summary> Проверка что за тегом следует его аргумент а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="tag">текущий тег</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега (в случае отсутствия - будет ошибка)</param>
        /// <returns>заданный тегом параметр</returns>
        public static string GetTagValueOrError(string[] args, string tag, bool ErrorIfNoValue = true)
        {
            var index = GetTagIndex(args, tag);
            return GetTagValueOrError(args, tag, index, ErrorIfNoValue);
        }
        /// <summary> Проверка что за тегом следуют его аргументы а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="tag">текущий тег</param>
        /// <param name="tag_index">индекс текущего тега</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега</param>
        /// <returns>заданный тегом параметр</returns>
        public static IEnumerable<string> GetTagMultipleValueOrError(string[] args, string tag, int tag_index, bool ErrorIfNoValue = true)
        {
            if (tag_index < 0 || tag_index >= args.Length)
                return ErrorIfNoValue ? throw new CLIHelpConfigurationException($"Key {tag} is not defined.") : null;

            CheckHelpArgAfterTag_PrintAndClose(args, tag, tag_index);

            var values = new List<string>();

            var first = GetIndexValueOrError(args, tag_index, ErrorIfNoValue, false);
            if (first is null)
                return null;

            values.Add(first);
            for (var i = tag_index + 2; i < args.Length; i++)
            {
                var value = args[i];
                if (value.StartsWith("-"))
                    break;
                values.Add(value);
            }
            return values;
        }

        /// <summary> Проверка что за тегом следуют его аргументы а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="tag">текущий тег</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега (в случае отсутствия - будет ошибка)</param>
        /// <returns>заданный тегом параметр</returns>
        public static IEnumerable<string> GetTagMultipleValueOrError(string[] args, string tag, bool ErrorIfNoValue = true)
        {
            var index = GetTagIndex(args, tag);
            return GetTagMultipleValueOrError(args, tag, index, ErrorIfNoValue);
        }



        #endregion

        #region Base Checks (Language | Help)

        /// <summary> Является ли тег идентификатором языка </summary>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool IsItLanguageTag(string tag) => tag is "-l" or "--lang" or "--language";
        /// <summary>
        /// Является ли тег help
        /// </summary>
        /// <param name="tag">тег</param>
        /// <returns></returns>
        public static bool IsItHelpTag(string tag) => tag is "-h" or "--help";

        #endregion

        #region Index

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

        #endregion

        #region Full Help
        /// <summary>
        /// Вывод на консоль доступных команд CLI
        /// </summary>
        /// <param name="lang">язык для вывода</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task PrintHelpInfoAsync(string lang, bool NeedCloseAfterPrint, string TagFilePath = null)
        {
            var help_tags = await GetLocalizedTagsAsync(lang, TagFilePath);

            PrintHelp(help_tags, NeedCloseAfterPrint);
        }
        /// <summary> Вывод на консоль доступных команд CLI </summary>
        /// <param name="lang">язык для вывода</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static void PrintHelpInfo(string lang, bool NeedCloseAfterPrint, string TagFilePath = null)
        {
            var help_tags = GetLocalizedTags(lang, TagFilePath);

            PrintHelp(help_tags, NeedCloseAfterPrint);
        }

        private static void PrintHelp(LocalizedTags help_tags, bool NeedCloseAfterPrint)
        {
            if (help_tags?.Tags is null)
            {
                "The program has no reference information".ConsoleRed();
                if (NeedCloseAfterPrint)
                    Environment.Exit(1);
                return;
            }

            foreach (var tag in help_tags.Tags)
                tag.ConsolePrint();

            if (NeedCloseAfterPrint)
                Environment.Exit(1);
        }

        /// <summary>
        /// Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task CheckForHelpTag_PrintAndCloseAsync(string[] args, bool NeedCloseAfterPrint = true, string TagFilePath = null)
        {
            if (args.Length <= 0 || !IsItHelpTag(args[0]))
                return;

            var lang = FindLanguageTagValue(args);
            await PrintSupportedLanguagesAsync(TagFilePath);
            await PrintHelpInfoAsync(lang, NeedCloseAfterPrint, TagFilePath); //en-Us, en, En - what lang or culture name you need
        }
        /// <summary>
        /// Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static void CheckForHelpTag_PrintAndClose(string[] args, bool NeedCloseAfterPrint = true, string TagFilePath = null)
        {
            if (args.Length <= 0 || !IsItHelpTag(args[0]))
                return;

            var lang = FindLanguageTagValue(args);
            PrintSupportedLanguages(TagFilePath);
            PrintHelpInfo(lang, NeedCloseAfterPrint, TagFilePath); //en-Us, en, En - what lang or culture name you need
        }

        #endregion

        #region Tag Info

        /// <summary> Вывод на консоль информации по тегу </summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static void PrintTagInfo(string tag, string lang = null, bool NeedCloseAfterPrint = true, string TagFilePath = null)
        {
            var help_tags = GetLocalizedTags(lang, TagFilePath);

            PrintTag(help_tags, tag, NeedCloseAfterPrint);
        }

        /// <summary> Вывод на консоль информации по тегу </summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task PringTagInfoAsync(string tag, string lang, bool NeedCloseAfterPrint = true, string TagFilePath = null)
        {
            var help_tags = await GetLocalizedTagsAsync(lang, TagFilePath);

            PrintTag(help_tags, tag, NeedCloseAfterPrint);
        }

        private static void PrintTag(LocalizedTags help_tags, string tag, bool NeedCloseAfterPrint)
        {
            if (help_tags?.FindTag(tag) is not { } find)
            {
                UnknownTagMessage(tag).ConsoleRed();
                if (NeedCloseAfterPrint)
                    Environment.Exit(1);
                return;
            }

            find.ConsolePrint();

            if (NeedCloseAfterPrint)
                Environment.Exit(1);
        }
        private static string UnknownTagMessage(string tag) => $"UNKNOWN tag: {tag}\nUse -h or --help for more information";

        #endregion

        #region Print Help if tag has help tag after him

        /// <summary> Выводит информацию по тегу и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <param name="tag_index">индекс аргумента</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static void CheckHelpArgAfterTag_PrintAndClose(string[] args, string tag, int tag_index, string TagFilePath = null)
        {
            var count = args.Length;
            if (tag_index < 0 || string.IsNullOrWhiteSpace(tag) || tag_index + 1 >= count)
                return;

            var arg = args[tag_index + 1];
            if (IsItHelpTag(arg))
            {

                var lang = IsItLanguageTag(tag) ? null : FindLanguageTagValue(args);


                var tag_help = new CLITags(lang, TagFilePath).CurrentCultureTags;
                PrintTag(tag_help, tag, true);
            }
            else if (IsItLanguageTag(tag) && tag_index + 2 < count && IsItHelpTag(args[tag_index + 2]))
            {
                PrintTagInfo(tag, arg, true, TagFilePath);
            }
        }
        /// <summary> Выводит информацию по тегу если за ним стоит тег -h или --help и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        public static void CheckHelpArgAfterTag_PrintAndClose(string[] args, string tag)
        {
            var index = GetTagIndex(args, tag);
            CheckHelpArgAfterTag_PrintAndClose(args, tag, index);
        }

        #endregion

        #region Find tag

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


        #endregion

        #region Get

        /// <summary> Получить локализованные теги </summary>
        /// <param name="lang">культура языка</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task<LocalizedTags> GetLocalizedTagsAsync(string lang, string TagFilePath = null) =>
            await Task.Run(() => GetLocalizedTags(lang, TagFilePath)).ConfigureAwait(false);

        /// <summary> Получить локализованные теги </summary>
        /// <param name="lang">культура языка</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static LocalizedTags GetLocalizedTags(string lang, string TagFilePath)
        {
            var cli = new CLITags(lang, TagFilePath);
            return cli.CurrentCultureTags;
        }

        #endregion

        #region Поддержка языков
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static async Task<IEnumerable<string>> GetSupportedLanguagesAsync(string TagFilePath = null) =>
            await Task.Run(() => GetSupportedLanguages(TagFilePath)).ConfigureAwait(false);
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static IEnumerable<string> GetSupportedLanguages(string TagFilePath = null)
        {
            var cli = new CLITags((CultureInfo)null, TagFilePath);
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

        /// <summary> Поиск языка в аргументах </summary>
        /// <param name="args">аргументы</param>
        /// <returns>параметр тега языка</returns>
        public static string FindLanguageTagValue(string[] args)
        {
            var lang_arg = args.FirstOrDefault(IsItLanguageTag);
            return lang_arg is not null ? GetTagValueOrError(args, lang_arg) : null;
        }


    }
}
