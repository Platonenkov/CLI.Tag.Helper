using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
        public static async Task WriteHelpInfo(string lang, string TagFilePath = null)
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
        /// <summary> Получить список локализованных тегов </summary>
        /// <param name="lang">культура языка</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns></returns>
        public static async Task<LocalizedTags> GetLocalizedTagsAsync(string lang, string TagFilePath = null) =>
            await Task.Run(() => GetLocalizedTags(lang,TagFilePath)).ConfigureAwait(false);

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
        public static async Task<IEnumerable<string>> GetSupportedLanguagesAsync(string TagFilePath = null) =>
            await Task.Run(()=>GetSupportedLanguages(TagFilePath)).ConfigureAwait(false);
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
            var languages = (await CLITagHelper.GetSupportedLanguagesAsync(TagFilePath)).ToArray();
            if (languages.Length > 0)
                $"Supported languages: {string.Join(", ", languages)}".ConsoleYellow();
        }
        /// <summary> Вывод на консоль поддерживаемых языков тегов </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public static void PrintSupportedLanguages(string TagFilePath = null)
        {
            var languages = CLITagHelper.GetSupportedLanguages(TagFilePath).ToArray();
            if (languages.Length > 0)
                $"Supported languages: {string.Join(", ", languages)}".ConsoleGreen();
        }

        #endregion

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
            GetLocalizedTags(TagFilePath);
            CurrentCulture = locale;
        }
        /// <summary> Перечисление локализованных тегов CLI </summary>
        public IReadOnlyCollection<LocalizedTags> LocalizedTags { get; private set; }
        /// <summary> Текущий язык или культура </summary>
        public string CurrentCulture { get; private set; }
        /// <summary> Устанавливает язык для получения локализованных тегов </summary>
        /// <param name="culture"></param>
        public void SetCulture(CultureInfo culture) => CurrentCulture = culture.Name;
        /// <summary> текущая локализация тегов </summary>
        public LocalizedTags CurrentCultureTags
        {
            get
            {
                var tags = LocalizedTags;
                var current_culture = CurrentCulture.ToUpper();
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

        #region Get Tags

        /// <summary> Путь к файлу с тегами </summary>
        private static string TagHelpFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{AppDomain.CurrentDomain.FriendlyName}.Tags.json");
        /// <summary> Получает перечисление известных тегов с описанием из файла </summary>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        /// <returns>перечисление тегов с описанием</returns>
        public void GetLocalizedTags(string TagFilePath = null)
        {
            var file_path = TagFilePath ?? TagHelpFilePath;
            var file = new FileInfo(file_path);
            if (!file.Exists) return;
            var json_file = File.ReadAllText(file_path);
            LocalizedTags = JsonSerializer.Deserialize<IReadOnlyCollection<LocalizedTags>>(json_file);
        }


        #endregion

        /// <summary> Перечисление поддерживаемых локализаций </summary>
        public IEnumerable<string> SupportedCultures => LocalizedTags.Select(t => t.Culture);
    }
}