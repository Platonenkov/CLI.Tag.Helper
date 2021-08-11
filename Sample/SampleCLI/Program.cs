﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CLI.Tag.Helper;

namespace SampleCLI
{
    class Program
    {
        public static CultureInfo CurrentCulture = CultureInfo.GetCultureInfo("en-Us");
        static async Task<int> Main(string[] args)
        {
            await CLITagHelper.PringTagInfoAsync("l", "ru");
            await CLITagHelper.PringTagInfoAsync("s", "ru");
            await CLITagHelper.CheckForHelpTagAndPrintAsync(args);
            Console.WriteLine(CLITagHelper.FindLanguageTagValue(args));

            CLITagHelper.PrintTagHelpInfoAndClose(args, "-s");
            CLITagHelper.PrintTagHelpInfoAndClose(args, "-l");
            CLITagHelper.CheckTagValueError(args, "l");
            var count = args.Length;
            foreach (var (arg, index) in args.Select((arg, i) => (arg, i)).Where(a => a.arg.StartsWith("-")))
                switch (arg)
                {
                    case "-h":
                    case "--help":
                        {
                            var lang_arg = args.FirstOrDefault(a => a is "-l" or "--lang" or "--language");
                            string lang = null;
                            if (lang_arg is not null && args.FirstIndexOf(lang_arg) is var lang_index)
                                lang = SetLanguage(lang_index);

                            await CLITagHelper.PrintSupportedLanguagesAsync();
                            await CLITagHelper.PrintHelpInfoAsync(lang);
                            await CLITagHelper.PringTagInfoAsync("b", lang);
                            await CLITagHelper.PringTagInfoAsync("h", lang);
                            return 1;
                        }
                    case "-l":
                    case "--lang":
                    case "--language":
                        {
                            SetLanguage(index);
                            break;
                        }

                }

            return 1;




            string SetLanguage(int lang_index)
            {
                if (lang_index == count - 1)
                    throw new ArgumentException($"Key {args[lang_index]} defined, but parameter is not defined.");

                var lang = args[lang_index + 1];
                CurrentCulture = CultureInfo.GetCultureInfo(lang);
                return lang;
            }
        }

    }
}
