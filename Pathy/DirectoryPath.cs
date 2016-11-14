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
        /// Instantiates a <see cref="DirectoryPath"/>.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">A relative directory path.</param>
        public DirectoryPath(DirectoryPath basePath, RelativeDirectoryPath relativePath)
            : base(basePath, relativePath)
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="DirectoryPath"/> from the given raw path.
        /// </summary>
        /// <param name="directoryPath">The path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="directoryPath"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException">There was a problem with the given path.</exception>
        /// <returns>A new <see cref="DirectoryPath"/> instance.</returns>
        public static new DirectoryPath From(string directoryPath)
        {
            Validation.CheckPath(directoryPath, nameof(directoryPath), Validations.Rooted);

            return new DirectoryPath(directoryPath);
        }

        /// <summary>
        /// Creates a directory in the temporary directory, and returns the path to it.
        /// </summary>
        /// <returns>A path to the temporary directory.</returns>
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
        /// Creates a <see cref="FilePath"/> from a base directory path and a relative file path.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">The relative file path.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static FilePath operator/(DirectoryPath basePath, RelativeFilePath relativePath) =>
            new FilePath(basePath, relativePath);

        /// <summary>
        /// Creates a <see cref="DirectoryPath"/> from a base directory path and a relative directory path.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">The relative directory path.</param>
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

        /// <inheritdoc />
        public sealed override bool Equals(object obj) => Equals(obj as DirectoryPath);

        /// <inheritdoc />
        public sealed override int GetHashCode() =>
            Comparer.GetHashCode(RawPath);

        /// <inheritdoc />
        public bool Equals(DirectoryPath other) =>
            Comparer.Equals(RawPath, other?.RawPath);

        /// <inheritdoc />
        public int CompareTo(DirectoryPath other) =>
            Comparer.Compare(RawPath, other?.RawPath);

        /// <summary>
        /// Compares two <see cref="DirectoryPath"/>s to see if they are considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are equal, otherwise <c>false</c>.</returns>
        public static bool operator ==(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) == 0;

        /// <summary>
        /// Compares two <see cref="DirectoryPath"/>s to see if they are not considered equal.
        /// </summary>
        /// <returns><c>true</c> if the paths are not equal, otherwise <c>false</c>.</returns>
        public static bool operator !=(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) != 0;

        /// 
        public static bool operator >(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) > 0;
        
        /// 
        public static bool operator <(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) < 0;
        
        /// 
        public static bool operator >=(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) >= 0;

        /// 
        public static bool operator <=(DirectoryPath left, DirectoryPath right) =>
            Comparer.Compare(left?.RawPath, right?.RawPath) <= 0;
    }
}
