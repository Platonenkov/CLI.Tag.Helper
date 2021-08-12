using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CLI.Tag.Helper.Exception;

namespace CLI.Tag.Helper
{
    public static class CliTagHelper
    {
        #region Arguments value

        /// <summary> Проверка что за тегом следует его аргумент а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="index">текущий индекс тега</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега (в случае отсутствия - будет ошибка)</param>
        /// <returns>заданный тегом параметр</returns>
        public static string GetIndexValueOrError(string[] args, int index, bool ErrorIfNoValue = true)
        {
            if (index == args.Length - 1 || index < 0 || args[index + 1].StartsWith("-"))
                return ErrorIfNoValue ? throw new CLIHelpConfigurationException($"Key {args[index]} defined, but parameter is not defined.") : null;
            return args[index + 1];
        }

        /// <summary> Проверка что за тегом следует его аргумент а не другой тег</summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="tag">текущий тег</param>
        /// <param name="tag_index">индекс текущего тега</param>
        /// <param name="ErrorIfNoValue">Обязательно наличие параметра у тега</param>
        /// <returns>заданный тегом параметр</returns>
        public static string GetTagValueOrError(string[] args, string tag, int tag_index, bool ErrorIfNoValue = true)
        {
            if (tag_index < 0 || tag_index >= args.Length)
                return ErrorIfNoValue ? throw new CLIHelpConfigurationException($"Key {tag} is not defined.") : null;

            CheckHelpArgAfterTag_PrintAndClose(args, tag, tag_index);

            return GetIndexValueOrError(args, tag_index, ErrorIfNoValue);
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

        /// <summary>
        /// Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <returns></returns>
        public static async Task CheckForHelpTag_PrintAndCloseAsync(string[] args, bool NeedCloseAfterPrint = true)
        {
            if (args.Length <= 0 || !IsItHelpTag(args[0]))
                return;

            var lang = FindLanguageTagValue(args);
            await PrintSupportedLanguagesAsync();
            await PrintHelpInfoAsync(lang); //en-Us, en, En - what lang or culture name you need
            if (NeedCloseAfterPrint)
                Environment.Exit(1);
        }
        /// <summary>
        /// Проверяет необходимость вывода help в консоль (если тег -h или --help является первым тегом)
        /// </summary>
        /// <param name="args">аргументы ком. строки</param>
        /// <param name="NeedCloseAfterPrint">Закрыть приложение если будет распечатан help</param>
        /// <returns></returns>
        public static void CheckForHelpTag_PrintAndClose(string[] args, bool NeedCloseAfterPrint = true)
        {
            if (args.Length <= 0 || !IsItHelpTag(args[0]))
                return;

            var lang = FindLanguageTagValue(args);
            PrintSupportedLanguages();
            PrintHelpInfo(lang); //en-Us, en, En - what lang or culture name you need
            if (NeedCloseAfterPrint)
                Environment.Exit(1);
        }

        #endregion

        #region Tag Info

        /// <summary>
        /// Выводит информацию по тегу и останавливает работу программы
        /// </summary>
        /// <param name="tag">тег</param>
        /// <param name="lang">язык</param>
        public static void PrintTagInfoAndClose(string tag, string lang = null)
        {
            PrintTagInfo(tag, lang);
            Environment.Exit(1);
        }
        /// <summary> Вывод на консоль информации по тегу </summary>
        /// <param name="tag">искомый тег</param>
        /// <param name="lang">язык для вывода</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static void PrintTagInfo(string tag, string lang = null, string TagFilePath = null)
        {
            var help_tags = GetLocalizedTags(lang, TagFilePath);

            if (help_tags?.FindTag(tag) is not { } find)
            {
                UnknownTagMessage(tag).ConsoleRed();
                return;
            }

            find.ConsolePrint();
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
                UnknownTagMessage(tag).ConsoleRed();
                return;
            }

            find.ConsolePrint();
        }
        private static string UnknownTagMessage(string tag) => $"UNKNOWN tag: {tag}\nUse -h or --help for more information";

        #endregion

        #region Print Help if tag has help tag after him

        /// <summary> Выводит информацию по тегу и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <param name="tag_index">индекс аргумента</param>
        public static void CheckHelpArgAfterTag_PrintAndClose(string[] args, string tag, int tag_index)
        {
            var count = args.Length;
            if (tag_index < 0 || string.IsNullOrWhiteSpace(tag) || tag_index + 1 >= count)
                return;

            var arg = args[tag_index + 1];
            if (IsItHelpTag(arg))
            {

                var lang = IsItLanguageTag(tag) ? null : FindLanguageTagValue(args);

                var tag_help = new CliTags(lang).CurrentCultureTags?.Tags?.FirstOrDefault(t => t.Names.Contains(tag.Trim('-')));
                tag_help?.ConsolePrint();
                Environment.Exit(1);
            }
            else if (IsItLanguageTag(tag) && tag_index + 2 < count && IsItHelpTag(args[tag_index + 2]))
            {
                PrintTagInfoAndClose(tag, arg);
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

        /// <summary> Выводит информацию по тегу если за ним стоит тег -h или --help и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        /// <param name="tag_index">индекс аргумента</param>
        public static void CheckHelpArgAfterTag_PrintAndClose(string[] args, char tag, int tag_index)
            => CheckHelpArgAfterTag_PrintAndClose(args, $"{tag}", tag_index);

        /// <summary> Выводит информацию по тегу если за ним стоит тег -h или --help и закрывает приложение </summary>
        /// <param name="args">аргументы</param>
        /// <param name="tag">тег</param>
        public static void CheckHelpArgAfterTag_PrintAndClose(string[] args, char tag)
            => CheckHelpArgAfterTag_PrintAndClose(args, $"{tag}");

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
            var cli = new CliTags(lang, TagFilePath);
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
            var cli = new CliTags((CultureInfo)null, TagFilePath);
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
