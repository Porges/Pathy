using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Pathy
{
    /// <summary>
    /// Represents an absolute path to a directory.
    /// </summary>
    public class DirectoryPath : AnyDirectoryPath, IEquatable<DirectoryPath>, IComparable<DirectoryPath>
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(Path.IsPathRooted(RawPath));
        }


        internal DirectoryPath(string dirPath)
            : base(dirPath)
        {
            Invariant();
        }

        /// <summary>
        /// Instantiates an <see cref="DirectoryPath"/>.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">A relative directory path.</param>
        public DirectoryPath(DirectoryPath basePath, RelativeDirectoryPath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }

        /// <summary>
        /// Creates an <see cref="DirectoryPath"/> from the given absolute path string.
        /// </summary>
        /// <param name="directoryPath">The absolute path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directoryPath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="directoryPath"/> is not an absolute path.</exception>
        /// <returns>An instance of <see cref="DirectoryPath"/>.</returns>
        public static new DirectoryPath From(string directoryPath)
        {
            Validation.CheckPath(directoryPath, nameof(directoryPath), Validations.Rooted);

            return new DirectoryPath(directoryPath);
        }

        public static DirectoryPath CreateTemporary()
        {
            var result = new DirectoryPath(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            result.Create();
            return result;
        }

        /// <inheritdoc/>
        /// <remarks>This is always <c>true</c>.</remarks>
        public sealed override bool IsAbsolute => true;

        /// <summary>
        /// Gets the path to the current working directory.
        /// </summary>
        /// <returns>The current working directory.</returns>
        public static DirectoryPath Current() => new DirectoryPath(Directory.GetCurrentDirectory());

        /// <summary>
        /// Combine the <paramref name="basePath"/> and <paramref name="relativePath"/> to create a new <see cref="FilePath"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static FilePath operator/(DirectoryPath basePath, RelativeFilePath relativePath) =>
            new FilePath(basePath, relativePath);

        /// <summary>
        /// Combine the <paramref name="basePath"/> and <paramref name="relativePath"/> to create a new <see cref="FilePath"/>.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="relativePath">The relative path.</param>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static DirectoryPath operator/(DirectoryPath basePath, RelativeDirectoryPath relativePath) =>
            new DirectoryPath(basePath, relativePath);

        /// <summary>
        /// Enumerates the files that exist in this directory.
        /// </summary>
        /// <returns>A lazily-enumerated list of files in the directory.</returns>
        public new IEnumerable<FilePath> EnumerateFiles()
        {
            foreach (var file in Directory.EnumerateFiles(RawPath))
            {
                yield return new FilePath(file);
            }
        }

        /// <summary>
        /// Enumerates the directories that exist in this directory.
        /// </summary>
        /// <returns>A lazily-enumerated list of directories in the directory.</returns>
        public new IEnumerable<DirectoryPath> EnumerateDirectories()
        {
            foreach (var dir in Directory.EnumerateDirectories(RawPath))
            {
                yield return new DirectoryPath(dir);
            }
        }

        public sealed override bool Equals(object obj) => Equals(obj as DirectoryPath);

        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        public bool Equals(DirectoryPath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        public int CompareTo(DirectoryPath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        public static bool operator ==(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        public static bool operator !=(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        public static bool operator >(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;

        public static bool operator <(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;

        public static bool operator >=(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        public static bool operator <=(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
