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
        /// <summary> комментарии к тегу </summary>
        public IEnumerable<string> Comments { get; init; }

        #endregion

        /// <returns>перечисление тегов с приставкой дефиса или дефисов в зависимости от длинны тега</returns>
        public IEnumerable<string> FullNames => Names.Select(GetFullTag);

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
            + (string.IsNullOrWhiteSpace(CommentsToString())
                ? null
                : $"\n{CommentsToString()}");

        /// <summary> Строковое составное значение комментариев </summary>
        /// <returns></returns>
        private string CommentsToString() => Comments is null ? null : string.Join("\n", Comments);
        /// <summary> Выводит информацию о теге на консоль </summary>
        public void ConsolePrint()
        {
            var names = Names.Aggregate(
                string.Empty,
                (
                    current,
                    tag) => current + $"\n{GetFullTag(tag),-20}");

            names.ConsoleGreen(string.IsNullOrWhiteSpace(Description));
            if (!string.IsNullOrWhiteSpace(Description))
                Description.ConsoleGreen();
            if (CommentsToString() is {Length:>0} comments)
                comments.ConsoleYellow();
        }
        #endregion

    }
}
