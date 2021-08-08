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

        public static async Task WriteHelpInfo(CultureInfo culture)
        {
            var help_tags = await GetLocalizedTagsAsync(culture);

            if (help_tags is null || help_tags.Tags is null)
            {
                "The program has no reference information".ConsoleRed();
                return;
            }

            foreach (var tag in help_tags.Tags)
                tag.ConsolePrint();
        }
        /// <summary> Получить список локализованных тегов </summary>
        /// <param name="culture">культура языка</param>
        /// <returns></returns>
        public static async Task<LocalizedTags> GetLocalizedTagsAsync(CultureInfo culture) =>
            await Task.Run(() => GetLocalizedTags(culture)).ConfigureAwait(false);

        /// <summary> Получить список локализованных тегов </summary>
        /// <param name="culture">культура языка</param>
        /// <returns></returns>
        public static LocalizedTags GetLocalizedTags(CultureInfo culture)
        {
            var cli = new CLITagHelper(culture);
            return cli.CurrentCultureTags;
        }
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        public static async Task<IEnumerable<string>> GetSupportedLanguagesAsync() => 
            await Task.Run(GetSupportedLanguages).ConfigureAwait(false);
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        public static IEnumerable<string> GetSupportedLanguages()
        {
            var cli = new CLITagHelper((CultureInfo) null);
            return cli.SupportedCultures;
        }

        public static async Task PrintSupportedLanguagesAsync()
        {
            var languages = (await CLITagHelper.GetSupportedLanguagesAsync()).ToArray();
            if (languages.Length > 0)
                $"Supported languages: {string.Join(", ", languages)}".ConsoleGreen();
        }
        public static void PrintSupportedLanguages()
        {
            var languages = CLITagHelper.GetSupportedLanguages().ToArray();
            if (languages.Length > 0)
                $"Supported languages: {string.Join(", ", languages)}".ConsoleGreen();
        }

        #endregion


        public CLITagHelper(CultureInfo culture) => Initialize(culture?.Name);

        public CLITagHelper(string locale) => Initialize(locale);

        private void Initialize(string locale)
        {
            GetLocalizedTags();
            CurrentCulture = locale;
        }
        /// <summary> Перечисление локализованных тегов CLI </summary>
        public IReadOnlyCollection<LocalizedTags> LocalizedTags { get; private set; }
        /// <summary> Текущий язык или культура </summary>
        public string CurrentCulture { get; private set; }

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
                    t => t.Culture?.ToUpper() is {Length:>0} current_tag
                         &&
                         (current_tag == current_culture ||
                          current_tag.Split('-')[0] == current_culture ||
                          current_tag == current_culture.Split('-')[0]));
                return current ?? tags.FirstOrDefault();
            }
        }

        #region Get Tags

        /// <summary> Путь к файлу с тегами </summary>
        private static string TagHelpFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CLITags.json");
        /// <summary> Получает перечисление известных тегов с описанием из файла </summary>
        /// <returns>перечисление тегов с описанием</returns>
        public void GetLocalizedTags()
        {
            var file_path = TagHelpFilePath;
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