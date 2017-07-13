using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using Pathy.Internal;

namespace Pathy
{
    /// <summary>
    /// Represents any path.
    /// </summary>
    public class AnyPath : IFormattable
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(RawPath != null);
            Debug.Assert(RawPath.IndexOfAny(Path.GetInvalidPathChars()) < 0);
        }

        /// <summary>
        /// The raw, <c>string</c>-based representation of the path.
        /// </summary>
        protected string RawPath { get; }

        // TODO: consider the Linux
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It is, in fact, immutable")]
        internal static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        internal AnyPath(string path)
        {
            RawPath = path;

            Invariant();
        }
        
        /// <summary>
        /// Creates an instance of <see cref="AnyPath"/> from the given path string.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="path"/> contains invalid characters.</exception>
        /// <returns>An instance of <see cref="AnyPath"/>.</returns>
        public static AnyPath From(string path)
        {
            Validation.CheckPath(path, nameof(path), Validations.Default);

            return new AnyPath(path);
        }
        
        /// <summary>
        /// Returns <c>true</c> if the path is absolute.
        /// </summary>
        public virtual bool IsAbsolute => Path.IsPathRooted(RawPath);

        /// <summary>
        /// Returns <c>true</c> if the path is relative.
        /// </summary>
        public bool IsRelative => !IsAbsolute;

        /// <summary>
        /// Gets the <see cref="FileAttributes"/> of the file or directory on the path.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public FileAttributes GetAttributes() => File.GetAttributes(RawPath);

        /// <summary>
        /// Gets the last time the file or directory was written to.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public DateTime GetLastWriteTimeLocal() => File.GetLastWriteTime(RawPath);

        /// <summary>
        /// Gets the last time the file or directory was written to.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public DateTime GetLastWriteTimeUtc() => File.GetLastWriteTimeUtc(RawPath);

        /// <summary>
        /// Gets the last time the file or directory was accessed.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public DateTime GetLastAccessTimeLocal() => File.GetLastAccessTime(RawPath);

        /// <summary>
        /// Gets the last time the file or directory was accessed.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public DateTime GetLastAccessTimeUtc() => File.GetLastAccessTimeUtc(RawPath);

        /// <summary>
        /// Gets the time the file or directory was created.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public DateTime GetCreationTimeLocal() => File.GetCreationTime(RawPath);

        /// <summary>
        /// Gets the time the file or directory was created.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not pure")]
        public DateTime GetCreationTimeUtc() => File.GetCreationTimeUtc(RawPath);

        /// <summary>
        /// Gets the raw representation of the path (as a string).
        /// </summary>
        /// <returns>The path.</returns>
        public sealed override string ToString() => RawPath;

        /// <summary>
        /// Formats the path as a string.
        /// </summary>
        /// <example>
        ///     <code>
        ///     var path = AnyPath.From(@"C:\path\to\a\file.txt");
        ///     var formatted = $"{path:20}";
        ///     </code>
        /// This results in <c>C:\...\file.txt</c>.
        /// </example>
        /// <param name="format">
        ///     The format string. Currently this can be a number, which
        ///     indicates how long the resulting string can be. If the path
        ///     is longer, it will be truncated and ellipses ("...") inserted.
        /// </param>
        /// <param name="formatProvider">A format provider. This is not used.</param>
        /// <returns>The path formatted per the format string.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return RawPath;
            }

            int width;

            const NumberStyles style = NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite;
            if (!int.TryParse(format, style, CultureInfo.InvariantCulture, out width) || width == 0 || width == int.MaxValue)
            {
                throw new FormatException("Unable to parse width");
            }

            if (width >= RawPath.Length)
            {
                return RawPath;
            }

            return SafeNativeMethods.CompactPathChecked(RawPath, width);
        }
    }
}
