using System;
using System.Diagnostics;
using System.IO;
using Pathy.Internal;

namespace Pathy
{
    /// <summary>
    /// Represents a filename.
    /// </summary>
    /// <remarks>This is represented as a relative file path with no directory part.</remarks>
    public class FileName : RelativeFilePath
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(RawPath.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
        }

        internal FileName(string fileName)
            : base(fileName)
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="FileName"/> from the given raw path.
        /// </summary>
        /// <param name="fileName">The path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fileName"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException">There was a problem with the given path.</exception>
        /// <returns>A new <see cref="FileName"/> instance.</returns>
        public new static FileName From(string fileName)
        {
            Validation.CheckPath(fileName, nameof(fileName), Validations.IsFileName);

            return new FileName(fileName);
        }

        /// <summary>
        /// Creates a new filename with the given extension.
        /// </summary>
        /// <param name="extension">The new extension (with or without a leading '.').</param>
        /// <remarks>Passing <c>null</c> will remove the extension.</remarks>
        /// <returns>The new filename.</returns>
        public new FileName WithExtension(string extension) =>
            new FileName(Path.ChangeExtension(RawPath, extension));

        /// <summary>
        /// Creates a new filename without any extension.
        /// </summary>
        /// <returns>The new filename.</returns>
        public new FileName WithoutExtension() =>
            new FileName(Path.ChangeExtension(RawPath, null));
    }
}
