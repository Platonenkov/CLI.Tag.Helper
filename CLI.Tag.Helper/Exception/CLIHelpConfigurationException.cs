using System.Runtime.Serialization;

namespace CLI.Tag.Helper.Exception
{
    public class CLIHelpConfigurationException : CLIHelpException
    {
        public CLIHelpConfigurationException() { }
        public CLIHelpConfigurationException(string message) : base(message) { }
        public CLIHelpConfigurationException(string message, System.Exception inner) : base(message, inner) { }

        protected CLIHelpConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
