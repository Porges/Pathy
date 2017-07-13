using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Pathy.Internal;

namespace Pathy
{
    /// <summary>
    ///  Represents a relative path to a directory.
    /// </summary>
    public class RelativeDirectoryPath 
        : AnyDirectoryPath
        , IEquatable<RelativeDirectoryPath>
        , IComparable<RelativeDirectoryPath>
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

        /// <summary>
        /// Instantiates a <see cref="RelativeDirectoryPath"/>.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">A relative directory path.</param>
        public RelativeDirectoryPath(RelativeDirectoryPath basePath, RelativeDirectoryPath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="RelativeDirectoryPath"/> from the given raw path.
        /// </summary>
        /// <param name="directoryPath">The path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="directoryPath"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException">There was a problem with the given path.</exception>
        /// <returns>A new <see cref="RelativeDirectoryPath"/> instance.</returns>
        public static new RelativeDirectoryPath From(string directoryPath)
        {
            Validation.CheckPath(directoryPath, nameof(directoryPath), Validations.NotRooted);

            return new RelativeDirectoryPath(directoryPath);
        }

        /// <summary>
        /// Creates a <see cref="RelativeFilePath"/> from a base directory path and a relative file path.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">The relative file path.</param>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static RelativeFilePath operator/(RelativeDirectoryPath basePath, RelativeFilePath relativePath) =>
            new RelativeFilePath(basePath, relativePath);

        /// <summary>
        /// Creates a <see cref="RelativeDirectoryPath"/> from a base directory path and a relative directory path.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">The relative directory path.</param>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static RelativeDirectoryPath operator /(RelativeDirectoryPath basePath, RelativeDirectoryPath relativePath) =>
            new RelativeDirectoryPath(basePath, relativePath);

        /// <inheritdoc />
        /// <remarks>This is always <c>false</c>.</remarks>
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
        
        /// <inheritdoc />
        public sealed override bool Equals(object obj) => Equals(obj as RelativeDirectoryPath);

        /// <inheritdoc />
        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        /// <inheritdoc />
        public bool Equals(RelativeDirectoryPath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        /// <inheritdoc />
        public int CompareTo(RelativeDirectoryPath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        /// <summary>
        /// Compares two <see cref="RelativeDirectoryPath"/>s to see if they are considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        /// <summary>
        /// Compares two <see cref="RelativeDirectoryPath"/>s to see if they are not considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        /// 
        public static bool operator >(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        /// 
        public static bool operator <(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        /// 
        public static bool operator >=(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        /// 
        public static bool operator <=(RelativeDirectoryPath left, RelativeDirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
