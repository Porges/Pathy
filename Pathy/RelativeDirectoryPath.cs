using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Pathy
{
    public class RelativeDirectoryPath : AnyDirectoryPath, IEquatable<RelativeDirectoryPath>, IComparable<RelativeDirectoryPath>
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(!Path.IsPathRooted(RawPath));
        }

        internal RelativeDirectoryPath(string rawPath)
            : base(rawPath)
        {
            Invariant();
        }

        public RelativeDirectoryPath(RelativeDirectoryPath basePath, RelativeDirectoryPath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }

        public static new RelativeDirectoryPath From(string directoryPath)
        {
            Validation.CheckPath(directoryPath, nameof(directoryPath), Validations.NotRooted);

            return new RelativeDirectoryPath(directoryPath);
        }

        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static RelativeFilePath operator/(RelativeDirectoryPath basePath, RelativeFilePath relativePath) =>
            new RelativeFilePath(basePath, relativePath);

        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static RelativeDirectoryPath operator /(RelativeDirectoryPath basePath, RelativeDirectoryPath relativePath) =>
            new RelativeDirectoryPath(basePath, relativePath);

        public sealed override bool IsAbsolute => false;

        /// <summary>
        /// Enumerates the files that exist in this directory.
        /// </summary>
        /// <returns>A lazily-enumerated list of files in the directory.</returns>
        public new IEnumerable<RelativeFilePath> EnumerateFiles()
        {
            foreach (var file in Directory.EnumerateFiles(RawPath))
            {
                yield return new RelativeFilePath(file);
            }
        }

        /// <summary>
        /// Enumerates the directories that exist in this directory.
        /// </summary>
        /// <returns>A lazily-enumerated list of directories in the directory.</returns>
        public new IEnumerable<RelativeDirectoryPath> EnumerateDirectories()
        {
            foreach (var dir in Directory.EnumerateDirectories(RawPath))
            {
                yield return new RelativeDirectoryPath(dir);
            }
        }

        public sealed override bool Equals(object obj) => Equals(obj as RelativeDirectoryPath);

        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        public bool Equals(RelativeDirectoryPath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        public int CompareTo(RelativeDirectoryPath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        public static bool operator ==(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        public static bool operator !=(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        public static bool operator >(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        public static bool operator <(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        public static bool operator >=(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        public static bool operator <=(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
