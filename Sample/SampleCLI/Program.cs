using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using CLI.Tag.Helper;

namespace SampleCLI
{
    class Program
    {
        public static CultureInfo CurrentCulture = CultureInfo.GetCultureInfo("en-Us");
        static async Task<int> Main(string[] args)
        {
            CLITagHelper.ConsolePrintTag = PrintTagHelp;
            CLITagHelper.ConsolePrintTags = PrintHelp;
            CLITagHelper.ConsolePrintArgs = PrintArguments;

            CLITagHelper.PrintArgumentsWithValues(args);
            await CLITagHelper.CheckForHelpTag_PrintAndCloseAsync(args);

            foreach (var (tag, index) in CLITagHelper.GetArguments(args))
                switch (tag)
                {
                    case "-l":
                    case "--lang":
                    case "--language":
                        {
                            var lang = CLITagHelper.GetTagValueOrError(args, tag, index);
                            CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                            Console.WriteLine($"Culture: {CurrentCulture?.Name}");
                            break;
                        }
                    default:
                        {
                            if (tag.StartsWith('-'))
                            {
                                var lang = CLITagHelper.FindLanguageTagValue(args);
                                CLITagHelper.PrintTagInfo(tag, lang);
                                return -1;
                            }
                            break;
                        }
                }

            return 1;
        }
        /// <summary>
        /// Выводит на консоль информацию по тегам CLI
        /// </summary>
        public static void PrintHelp(IEnumerable<Tag> tags)
        {
            var doc = new Document(new Grid
            {
                Stroke = LineThickness.Double,
                StrokeColor = ConsoleColor.Green,
                Columns =
                {
                    new Column
                    {
                        Width = GridLength.Auto, MinWidth = 2,
                    },
                    new Column
                    {
                        Width = GridLength.Auto
                    },
                    new Column
                    {
                        Width = GridLength.Star(1)
                    }
                },
                Children =
                {
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow, TextAlign = TextAlign.Center,ColumnSpan = 3,
                        Children = {"HELP"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow, TextAlign = TextAlign.Center,
                        Children = {"#"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,TextAlign = TextAlign.Center,
                        Children = {"Tag"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,TextAlign = TextAlign.Center,
                        Children = {"Description"}
                    },
                    tags?.Select((tag, i) => GetTagCells(tag,i))
                }
            });
            ConsoleRenderer.RenderDocument(doc);
        }
        /// <summary>
        /// Выводит на консоль информацию по тегам CLI
        /// </summary>
        public static void PrintTagHelp(Tag tag)
        {
            var doc = new Document(new Grid
            {
                Stroke = LineThickness.Double,
                StrokeColor = ConsoleColor.Green,
                Columns =
                {
                    new Column
                    {
                        Width = GridLength.Auto
                    },
                    new Column
                    {
                        Width = GridLength.Star(1)
                    }
                },
                Children =
                {
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,TextAlign = TextAlign.Center,
                        Children = {"Tag"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,TextAlign = TextAlign.Center,
                        Children = {"Description"}
                    },
                    GetTagCells(tag)
                }
            });
            ConsoleRenderer.RenderDocument(doc);
        }

        private static IEnumerable<Cell> GetTagCells(Tag tag, int index) =>
            new[]
            {
                new Cell
                {
                    Stroke = LineThickness.Single,
                    Color = ConsoleColor.Yellow,
                    Margin = new Thickness(1, 0),
                    VerticalAlign = VerticalAlign.Center,
                    Children = { index }
                },
                new Cell
                {
                    Stroke = LineThickness.Single,
                    Color = ConsoleColor.White,
                    TextWrap = TextWrap.WordWrap,
                    Margin = new Thickness(1, 0),
                    TextAlign = TextAlign.Right,
                    VerticalAlign = VerticalAlign.Center,
                    Children = {string.Join("\n", tag.FullNames)}
                },
                new Cell
                {
                    Stroke = LineThickness.Single,
                    Color = ConsoleColor.White,
                    TextWrap = TextWrap.WordWrap,
                    Margin = new Thickness(1, 0),
                    VerticalAlign = VerticalAlign.Center,
                    Children = {tag.Description}
                },
                tag.Comments is null
                    ? null
                    : new Cell
                    {
                        ColumnSpan = 3,
                        Stroke = LineThickness.Single,
                        Color = ConsoleColor.DarkYellow,
                        TextWrap = TextWrap.WordWrap,
                        Margin = new Thickness(1, 0),
                        Children = {string.Join("\n", tag.Comments)}
                    }

            };

        private static IEnumerable<Cell> GetTagCells(Tag tag) =>
            new[]
            {
                new Cell
                {
                    Stroke = LineThickness.Single,
                    Color = ConsoleColor.White,
                    TextWrap = TextWrap.WordWrap,
                    Margin = new Thickness(1, 0),
                    TextAlign = TextAlign.Right,
                    VerticalAlign = VerticalAlign.Center,
                    Children = {string.Join("\n", tag.FullNames)}
                },
                new Cell
                {
                    Stroke = LineThickness.Single,
                    Color = ConsoleColor.White,
                    TextWrap = TextWrap.WordWrap,
                    Margin = new Thickness(1, 0),
                    VerticalAlign = VerticalAlign.Center,
                    Children = {tag.Description}
                },
                tag.Comments is null
                    ? null
                    : new Cell
                    {
                        ColumnSpan = 2,
                        Stroke = LineThickness.Single,
                        Color = ConsoleColor.DarkYellow,
                        TextWrap = TextWrap.WordWrap,
                        Margin = new Thickness(1, 0),
                        Children = {string.Join("\n", tag.Comments)}
                    }

            };

        /// <summary>
        /// Выводим список заданных аргументов и параметров к ним
        /// </summary>
        public static void PrintArguments(IEnumerable<(string tag, int index, string parameters)> rows)
        {
            var doc = new Document(new Grid
            {
                Stroke = LineThickness.Double,
                StrokeColor = ConsoleColor.Green,
                Columns =
                {
                    new Column
                    {
                        Width = GridLength.Auto, MinWidth = 2
                    },
                    new Column
                    {
                        Width = GridLength.Auto
                    },
                    new Column
                    {
                        Width = GridLength.Star(1)
                    }
                },
                Children =
                {
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow, TextAlign = TextAlign.Center,ColumnSpan = 3,
                        Children = {"Arguments"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow, TextAlign = TextAlign.Center,
                        Children = {"#"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,TextAlign = TextAlign.Center,
                        Children = {"Tag"}
                    },
                    new Cell
                    {
                        Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,TextAlign = TextAlign.Center,
                        Children = {"Values"}
                    },
                    rows.Select((r,i) => new[]
                    {
                        new Cell
                        {
                            Stroke = LineThickness.Single, Color = ConsoleColor.Yellow,Margin = new Thickness(1,0),
                            Children = {i}
                        },
                        new Cell
                        {
                            Stroke = LineThickness.Single, Color = ConsoleColor.White,
                            TextWrap = TextWrap.WordWrap,
                            Margin = new Thickness(1,0),
                            TextAlign = TextAlign.Right,
                            Children = {r.tag}
                        },
                        new Cell
                        {
                            Stroke = LineThickness.Single, Color = ConsoleColor.White, TextWrap = TextWrap.WordWrap,Margin = new Thickness(1,0),
                            Children = {r.parameters}
                        },
                    })
                }
            });
            ConsoleRenderer.RenderDocument(doc);
        }

    }
}
