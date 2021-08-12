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
            var rows = CLITagHelper.GetArgumentsWithOneStringValues(args).ToArray();
            if (rows.Length > 0)
            {
                "Arguments:".ConsoleYellow();
                var grid = new Document(new Grid { Stroke = LineThickness.Single, StrokeColor = ConsoleColor.Green }
                   .AddColumns(
                        new Column { Width = GridLength.Auto },
                        new Column { Width = GridLength.Star(1) }
                    )
                   .AddChildren(
                        new Cell { Stroke = LineThickness.Single, Color = ConsoleColor.White }
                           .AddChildren("Tag"),
                        new Cell { Stroke = LineThickness.Single, Color = ConsoleColor.White }
                           .AddChildren("Values"),
                        rows.Select(
                            r => new[]
                            {
                                new Cell {Stroke = LineThickness.Single, Color = ConsoleColor.Yellow, TextWrap = TextWrap.WordWrap}
                                   .AddChildren(r.tag),
                                new Cell {Stroke = LineThickness.Single, Color = ConsoleColor.White, TextWrap = TextWrap.WordWrap}
                                   .AddChildren(r.parameters),
                            })
                    ));
                ConsoleRenderer.RenderDocument(grid);
            }

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

    }
}
