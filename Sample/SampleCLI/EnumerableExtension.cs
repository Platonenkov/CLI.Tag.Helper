using System.Collections.Generic;

namespace SampleCLI
{
    public static class EnumerableExtension
    {

        /// <summary>Индекс первого найденного элемента</summary>
        /// <typeparam name="T">Тип искомого элемента</typeparam>
        /// <param name="enumerable">Перечисление элементов, в котором требуется найти заданный</param>
        /// <param name="item">Искомый элемент</param>
        /// <returns>Индекс первого вхождения элемента в перечисление, либо -1 в случае, если он в ней отсутствует</returns>
        public static int FirstIndexOf<T>(this IEnumerable<T> enumerable, in T item)
        {
            int num = -1;
            foreach (T obj in enumerable)
            {
                ++num;
                if (object.Equals((object)obj, (object)item))
                    return num;
            }
            return num;
        }
    }
}