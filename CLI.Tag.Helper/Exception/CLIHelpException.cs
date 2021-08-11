using System;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;

namespace CLI.Tag.Helper.Exception
{
    [Serializable]
    public class CLIHelpException : ApplicationException
    {
        public static void Handle(System.Exception error)
        {
            var exception = error;
            while (exception != null)
            {
                Console.WriteLine(exception.Message);
                foreach (var data in error.Data.Cast<DictionaryEntry>())
                    Console.WriteLine("{0}:{1}", data.Key, data.Value);

                exception = exception.InnerException;
            }
        }

        public bool Handle()
        {
            Handle(this);
            return true;
        }

        public CLIHelpException() { }
        public CLIHelpException(string message) : base(message) { }
        public CLIHelpException(string message, System.Exception inner) : base(message, inner) { }

        protected CLIHelpException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}