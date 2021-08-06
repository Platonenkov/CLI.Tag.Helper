using System.Collections;
using System.Collections.Generic;

namespace CLI.Tag.Helper
{
    public class LocalizedTags
        //: IEnumerable
    {
        /// <summary> Локализация </summary>
        public string Culture { get; init; }
        /// <summary> Список тегов </summary>
        public IEnumerable<Tag> Tags { get; init; }

        //public IEnumerator GetEnumerator() => Tags.GetEnumerator();
    }
}