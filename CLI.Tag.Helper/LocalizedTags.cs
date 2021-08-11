using System.Collections.Generic;
using System.Linq;

namespace CLI.Tag.Helper
{
    public class LocalizedTags
    {
        /// <summary> Локализация </summary>
        public string Culture { get; init; }
        /// <summary> Список тегов </summary>
        public IEnumerable<Tag> Tags { get; init; }

        public Tag FindTag(string tag) => tag?.Trim('-') is { Length: > 0 } t ? Tags?.FirstOrDefault(v => v.Names.Contains(t.TrimStart('-'))) : null;
    }
}