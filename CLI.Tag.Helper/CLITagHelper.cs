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
        public static async Task<LocalizedTags> GetLocalizedTagsAsync(CultureInfo culture) =>
            await Task.Run(() =>
            {
                var cli = new CLITagHelper(culture);
                return cli.CurrentCultureTags;
            }).ConfigureAwait(false);
        public static LocalizedTags GetLocalizedTags(CultureInfo culture)
        {
            var cli = new CLITagHelper(culture);
            return cli.CurrentCultureTags;
        }

        public CLITagHelper(CultureInfo culture)
        {
            GetLocalizedTags();
            CurrentCulture = culture.Name;
        }
        public CLITagHelper(string locale)
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
        public LocalizedTags CurrentCultureTags =>
            string.IsNullOrWhiteSpace(CurrentCulture)
                ? LocalizedTags.FirstOrDefault()
                : LocalizedTags.FirstOrDefault(t => 
                    t.Culture.ToUpper() == CurrentCulture.ToUpper() ||
                    t.Culture.ToUpper() == CurrentCulture.Split('-')[0].ToUpper());

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