using System;
using System.Diagnostics;
using System.IO;
using Pathy.Internal;

namespace Pathy
{
    /// <summary>
    /// Represents a relative path to a file.
    /// </summary>
    public class RelativeFilePath
        : AnyFilePath
        , IEquatable<RelativeFilePath>
        , IComparable<RelativeFilePath>
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(!Path.IsPathRooted(RawPath));
        }

        internal RelativeFilePath(string filePath)
            : base(filePath)
        {
            Invariant();
        }

        /// <summary>
        /// Instantiates a <see cref="FilePath"/>.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">A relative file path.</param>
        public RelativeFilePath(RelativeDirectoryPath basePath, RelativeFilePath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="RelativeFilePath"/> from the given raw path.
        /// </summary>
        /// <param name="filePath">The path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="filePath"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException">There was a problem with the given path.</exception>
        /// <returns>A new <see cref="RelativeFilePath"/> instance.</returns>
        public static new RelativeFilePath From(string filePath)
        {
            Validation.CheckPath(filePath, nameof(filePath), Validations.NotRooted|Validations.IsFile);

            return new RelativeFilePath(filePath);
        }

        /// <inheritdoc />
        /// <remarks>This is always <c>false</c>.</remarks>
        public sealed override bool IsAbsolute => false;
        
        /// <summary>
        /// Creates a new file path with the given extension.
        /// </summary>
        /// <param name="extension">The new extension (with or without a leading '.').</param>
        /// <remarks>Passing <c>null</c> will remove the extension.</remarks>
        /// <returns>The new file path.</returns>
        public new RelativeFilePath WithExtension(string extension) =>
            new RelativeFilePath(Path.ChangeExtension(RawPath, extension));

        /// <summary>
        /// Creates a new file path without any extension.
        /// </summary>
        /// <returns>The new file path.</returns>
        public new RelativeFilePath WithoutExtension() =>
            new RelativeFilePath(Path.ChangeExtension(RawPath, null));

        /// <inheritdoc />
        public sealed override bool Equals(object obj) => Equals(obj as RelativeFilePath);

        /// <inheritdoc />
        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        /// <inheritdoc />
        public bool Equals(RelativeFilePath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        /// <inheritdoc />
        public int CompareTo(RelativeFilePath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        /// <summary>
        /// Compares two <see cref="RelativeFilePath"/>s to see if they are considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        /// <summary>
        /// Compares two <see cref="RelativeFilePath"/>s to see if they are not considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        /// 
        public static bool operator >(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        /// 
        public static bool operator <(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        /// 
        public static bool operator >=(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        /// 
        public static bool operator <=(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
