using System;
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
            await CliTagHelper.CheckForHelpTag_PrintAndCloseAsync(args);

            foreach (var (tag, index) in args.Select((arg, i) => (arg, i)).Where(a => a.arg.StartsWith("-")))
                switch (tag)
                {
                    case "-l":
                    case "--lang":
                    case "--language":
                        {
                            var lang = CliTagHelper.GetTagValueOrError(args, tag, index);
                            CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                            Console.WriteLine($"Culture: {CurrentCulture?.Name}");
                            break;
                        }
                    default:
                        {
                            if (tag.StartsWith('-'))
                            {
                                var lang = CliTagHelper.FindLanguageTagValue(args);
                                CliTagHelper.PrintTagInfo(tag, lang);
                                return -1;
                            }
                            break;
                        }
                }

            return 1;
        }

    }
}
