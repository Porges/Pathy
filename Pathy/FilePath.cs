using System;
using System.Diagnostics;
using System.IO;

namespace Pathy
{
    /// <summary>
    /// Represents an absolute path to a file.
    /// </summary>
    public sealed class FilePath : AnyFilePath, IEquatable<FilePath>, IComparable<FilePath>
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

        public FilePath(DirectoryPath basePath, RelativeFilePath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }
        
        public new static FilePath From(string filePath)
        {
            Validation.CheckPath(filePath, nameof(filePath), Validations.Rooted | Validations.IsFile);

            return new FilePath(filePath);
        }

        /// <inheritdoc />
        /// <remarks>This is always <c>true</c>.</remarks>
        public sealed override bool IsAbsolute => true;

        public static FilePath CreateTemporary() =>
            new FilePath(Path.GetTempFileName());

        /// <summary>
        /// Gets the parent <see cref="DirectoryPath"/> for the file.
        /// </summary>
        public DirectoryPath Directory =>
            new DirectoryPath(Path.GetDirectoryName(RawPath));

        public new FilePath WithExtension(string extension) =>
            new FilePath(Path.ChangeExtension(RawPath, extension));

        public new FilePath WithoutExtension() =>
            new FilePath(Path.ChangeExtension(RawPath, null));

        public sealed override bool Equals(object obj) => Equals(obj as FilePath);

        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        public bool Equals(FilePath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        public int CompareTo(FilePath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        public static bool operator ==(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        public static bool operator !=(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        public static bool operator >(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        public static bool operator <(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        public static bool operator >=(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        public static bool operator <=(FilePath left, FilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
