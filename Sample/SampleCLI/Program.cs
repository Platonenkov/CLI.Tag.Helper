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
            var count = args.Length;
            foreach (var (arg, index) in args.Select((arg, i) => (arg, i)).Where(a => a.arg.StartsWith("-")))
                switch (arg)
                {
                    case "-h":
                    case "--help":
                        {
                            var lang_arg = args.FirstOrDefault(a => a is "-l" or "--lang" or "--language");
                            if (lang_arg is not null && args.FirstIndexOf(lang_arg) is var lang_index)
                                SetLanguage(lang_index);

                            await CLITagHelper.PrintSupportedLanguagesAsync();
                            await CLITagHelper.WriteHelpInfo(CurrentCulture);
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




            void SetLanguage(int lang_index)
            {
                if (lang_index == count - 1)
                    throw new ArgumentException($"Key {args[lang_index]} defined, but parameter is not defined.");

                var lang = args[lang_index + 1];
                CurrentCulture = CultureInfo.GetCultureInfo(lang);
            }
        }

    }
}
