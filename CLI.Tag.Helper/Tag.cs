using System;
using System.Collections.Generic;
using System.Linq;

namespace CLI.Tag.Helper
{
    public class Tag
    {
        #region Tag

        /// <summary> Список тегов выполняющих одну функцию </summary>
        public IEnumerable<string> Names { get; init; }
        /// <summary> описание действий тега </summary>
        public string Description { get; init; }
        /// <summary> комментарий к тегу </summary>
        public string Comment { get; init; }

        #endregion


        /// <summary>
        /// получает тег с приставкой дефиса или дефисов в зависимости от длинны тега
        /// </summary>
        /// <param name="tag">тег</param>
        /// <returns>тег с приставкой дефиса или дефисов в зависимости от длинны тега</returns>
        private static string GetFullTag(string tag) => tag.Length == 1 ? $"-{tag}" : $"--{tag}";

        #region Overrides of Object

        public override string ToString() =>
            Names.Aggregate(
                string.Empty,
                (
                    current,
                    tag) => current + $"\n{GetFullTag(tag),-20}")
            + Description
            + (string.IsNullOrWhiteSpace(Comment)
                ? null
                : $"\n{Comment}")
            + Environment.NewLine;

        #endregion

    }
}
