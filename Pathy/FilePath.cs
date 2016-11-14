using System;
using System.Diagnostics;
using System.IO;

namespace Pathy
{
    /// <summary>
    /// Represents an absolute path to a file.
    /// </summary>
    public sealed class FilePath 
        : AnyFilePath
        , IEquatable<FilePath>
        , IComparable<FilePath>
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(Path.IsPathRooted(RawPath));
        }

        internal FilePath(string rawPath)
            : base(rawPath)
        {
            Invariant();
        }

        /// <summary>
        /// Instantiates a <see cref="FilePath"/>.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">A relative file path.</param>
        public FilePath(DirectoryPath basePath, RelativeFilePath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }
        
        /// <summary>
        /// Creates a <see cref="FilePath"/> from the given raw path.
        /// </summary>
        /// <param name="filePath">The path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="filePath"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException">There was a problem with the given path.</exception>
        /// <returns>A new <see cref="FilePath"/> instance.</returns>
        public new static FilePath From(string filePath)
        {
            Validation.CheckPath(filePath, nameof(filePath), Validations.Rooted | Validations.IsFile);

            return new FilePath(filePath);
        }

        /// <inheritdoc />
        /// <remarks>This is always <c>true</c>.</remarks>
        public sealed override bool IsAbsolute => true;

        /// <summary>
        /// Creates a new empty file in the temporary directory and returns the path to it.
        /// </summary>
        /// <returns>An absolute path to the new file.</returns>
        public static FilePath CreateTemporary() =>
            new FilePath(Path.GetTempFileName());

        /// <summary>
        /// Gets the parent <see cref="DirectoryPath"/> for the file.
        /// </summary>
        public DirectoryPath Directory =>
            new DirectoryPath(Path.GetDirectoryName(RawPath));

        /// <summary>
        /// Creates a new file path with the given extension.
        /// </summary>
        /// <param name="extension">The new extension (with or without a leading '.').</param>
        /// <remarks>Passing <c>null</c> will remove the extension.</remarks>
        /// <returns>The new file path.</returns>
        public new FilePath WithExtension(string extension) =>
            new FilePath(Path.ChangeExtension(RawPath, extension));

        /// <summary>
        /// Creates a new file path without any extension.
        /// </summary>
        /// <returns>The new file path.</returns>
        public new FilePath WithoutExtension() =>
            new FilePath(Path.ChangeExtension(RawPath, null));
        
        /// <inheritdoc />
        public sealed override bool Equals(object obj) => Equals(obj as FilePath);

        /// <inheritdoc />
        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        /// <inheritdoc />
        public bool Equals(FilePath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        /// <inheritdoc />
        public int CompareTo(FilePath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        /// <summary>
        /// Compares two <see cref="FilePath"/>s to see if they are considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        /// <summary>
        /// Compares two <see cref="FilePath"/>s to see if they are not considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        /// 
        public static bool operator >(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        /// 
        public static bool operator <(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        /// 
        public static bool operator >=(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        /// 
        public static bool operator <=(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
