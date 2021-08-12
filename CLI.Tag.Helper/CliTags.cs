using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CLI.Tag.Helper
{
    public class CLITags
    {

        #region Конструкторы

        /// <summary> конструктор </summary>
        /// <param name="culture">культура для поиска тегов</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public CLITags(CultureInfo culture, string TagFilePath = null) => Initialize(culture?.Name, TagFilePath);
        /// <summary> конструктор </summary>
        /// <param name="locale">название языка для поиска тегов</param>
        /// <param name="TagFilePath">путь к файлу тегов (если Null - используется PROJECT_NAME.Tags.json)</param>
        public CLITags(string locale, string TagFilePath = null) => Initialize(locale, TagFilePath);

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
        /// <summary> Перечисление поддерживаемых локализаций </summary>
        public IEnumerable<string> SupportedCultures => LocalizedTags.Select(t => t.Culture);

        /// <summary> Устанавливает язык для получения локализованных тегов </summary>
        /// <param name="culture"></param>
        public void ChangeCulture(CultureInfo culture) => CurrentCulture = culture?.Name;

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

    }
}