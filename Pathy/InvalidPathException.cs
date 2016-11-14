using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using static System.FormattableString;

namespace Pathy
{
    /// <summary>
    /// This exception is thrown when you attempt to create any
    /// path type with an invalid path string.
    /// </summary>
    [Serializable]
    public class InvalidPathException : ArgumentException
    {
        private static string FormatMessage(char c) =>
            Invariant($"Path contains invalid character: '{c}' (U+{(int)c:X4})");

        /// <summary>
        /// Creates an instance of <see cref="InvalidPathException"/>.
        /// </summary>
        public InvalidPathException()
            : base("Path contains invalid character")
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidPathException"/>,
        /// with a custom message.
        /// </summary>
        /// <param name="message">The message to use.</param>
        public InvalidPathException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidPathException"/>,
        /// with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The message to use.</param>
        /// <param name="innerException">The inner exception to use.</param>
        public InvalidPathException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an instance of <see cref="InvalidPathException"/> with
        /// a message based upon the invalid character and parameter name.
        /// </summary>
        /// <param name="invalidCharacter">The (first) character that caused
        ///     the path to be invalid.</param>
        /// <param name="paramName">The parameter name of the calling method.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "param", Justification = "Matches existing parameter name")]
        public InvalidPathException(char invalidCharacter, string paramName)
            : base(FormatMessage(invalidCharacter), paramName)
        {
        }
        
        /// <inheritdoc />
        protected InvalidPathException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
