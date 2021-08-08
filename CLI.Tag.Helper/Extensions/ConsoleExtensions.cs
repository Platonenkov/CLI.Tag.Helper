using System.Diagnostics;

namespace System
{
    public static class ConsoleExtensions
    {
        /// <summary>
        /// Writes green text to the console.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="newLine">print with new line after</param>
        [DebuggerStepThrough]
        public static void ConsoleGreen(this string text, bool newLine = true)
        {
            text.ColoredWriteLine(ConsoleColor.Green, newLine);
        }

        /// <summary>
        /// Writes red text to the console.
        /// </summary>
        /// <param name="newLine">print with new line after</param>
        /// <param name="text">The text.</param>
        [DebuggerStepThrough]
        public static void ConsoleRed(this string text, bool newLine = true)
        {
            text.ColoredWriteLine(ConsoleColor.Red, newLine);
        }

        /// <summary>
        /// Writes yellow text to the console.
        /// </summary>
        /// <param name="newLine">print with new line after</param>
        /// <param name="text">The text.</param>
        [DebuggerStepThrough]
        public static void ConsoleYellow(this string text, bool newLine = true)
        {
            text.ColoredWriteLine(ConsoleColor.Yellow, newLine);
        }

        /// <summary>
        /// Writes out text with the specified ConsoleColor.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="newLine">print with new line after</param>
        [DebuggerStepThrough]
        public static void ColoredWriteLine(this string text, ConsoleColor color, bool newLine = true)
        {
            Console.ForegroundColor = color;
            if (newLine)
                Console.WriteLine(text);
            else
                Console.Write(text);
            Console.ResetColor();
        }
    }
}
