using System;
using System.Diagnostics;
using System.IO;

namespace Pathy
{
    public class RelativeFilePath : AnyFilePath, IEquatable<RelativeFilePath>, IComparable<RelativeFilePath>
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

        public RelativeFilePath(RelativeDirectoryPath path, RelativeFilePath fileName)
            : base(path, fileName)
        {
            Invariant();
        }

        public static new RelativeFilePath From(string filePath)
        {
            Validation.CheckPath(filePath, nameof(filePath), Validations.NotRooted|Validations.IsFile);

            return new RelativeFilePath(filePath);
        }

        /// <inheritdoc />
        /// <remarks>This is always <c>false</c>.</remarks>
        public sealed override bool IsAbsolute => false;

        public sealed override bool Equals(object obj) => Equals(obj as FilePath);

        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        public bool Equals(RelativeFilePath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        public int CompareTo(RelativeFilePath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        public static bool operator ==(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        public static bool operator !=(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        public static bool operator >(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        public static bool operator <(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        public static bool operator >=(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        public static bool operator <=(RelativeFilePath left, RelativeFilePath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
