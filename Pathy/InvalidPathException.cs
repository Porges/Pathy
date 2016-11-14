using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using static System.FormattableString;

namespace Pathy
{
    [Serializable]
    public class InvalidPathException : ArgumentException
    {
        private static string FormatMessage(char c) =>
            Invariant($"Path contains invalid character: '{c}' (U+{(int)c:X4})");

        public InvalidPathException()
            : base("Path contains invalid character")
        {
        }

        public InvalidPathException(string message)
            : base(message)
        {
        }

        public InvalidPathException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "param", Justification = "Matches existing parameter name")]
        public InvalidPathException(char invalidCharacter, string paramName)
            : base(FormatMessage(invalidCharacter), paramName)
        {
        }
        
        protected InvalidPathException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
