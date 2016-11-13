using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.AccessControl;

namespace Pathy
{
    /// <summary>
    /// Represents a relative or absolute directory path.
    /// </summary>
    public class AnyDirectoryPath : AnyPath
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            // no extra invariants, yet
            Debug.Assert(this != null);
        }

        internal AnyDirectoryPath(string directoryPath)
            : base(directoryPath)
        {
            Invariant();
        }

        public AnyDirectoryPath(AnyDirectoryPath basePath, RelativeDirectoryPath relativePath)
            : base(Combined(basePath, relativePath))
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="AnyDirectoryPath"/> from the given directory path.
        /// </summary>
        /// <param name="directoryPath">The directory path to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="directoryPath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="directoryPath"/> contains invalid characters.</exception>
        /// <returns>An instance of <see cref="AnyDirectoryPath"/>.</returns>
        public static new AnyDirectoryPath From(string directoryPath)
        {
            Validation.CheckPath(directoryPath, nameof(directoryPath), Validations.Default);

            return new AnyDirectoryPath(directoryPath);
        }

        private static string Combined(AnyDirectoryPath basePath, RelativeDirectoryPath relativePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }


            var result = Path.Combine(basePath.RawPath, relativePath.RawPath);

            // if we have an absolute path we normalize it,
            // so we can have consistent comparison
            return basePath.IsAbsolute ? Path.GetFullPath(result) : result;
        }
        
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static AnyFilePath operator/(AnyDirectoryPath basePath, RelativeFilePath relativePath) =>
            new AnyFilePath(basePath, relativePath);

        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Alternate is provided as constructor")]
        public static AnyDirectoryPath operator/(AnyDirectoryPath basePath, RelativeDirectoryPath file) =>
            new AnyDirectoryPath(basePath, file);

        /// <summary>
        /// Creates the directory (including any parent directories), 
        /// unless it already exists.
        /// </summary>
        public void Create() => Directory.CreateDirectory(RawPath);

        public void Delete() => Directory.Delete(RawPath, true);
        
        public void Delete(bool recursive) => Directory.Delete(RawPath, recursive);
        
        /// <summary>
        /// Creates the directory (including any parent directories), 
        /// unless it already exists.
        /// </summary>
        /// <param name="directorySecurity">The Windows security to apply.</param>
        public void Create(DirectorySecurity directorySecurity) =>
            Directory.CreateDirectory(RawPath, directorySecurity);

        /// <summary>
        /// Gets the directory name as a relative path.
        /// </summary>
        public RelativeDirectoryPath DirectoryName => new RelativeDirectoryPath(Path.GetFileName(RawPath));

        /// <summary>
        /// Enumerates the files that exist in this directory.
        /// </summary>
        /// <returns>A lazily-enumerated list of files in the directory.</returns>
        public IEnumerable<AnyFilePath> EnumerateFiles()
        {
            foreach (var file in Directory.EnumerateFiles(RawPath))
            {
                yield return new AnyFilePath(file);
            }
        }

        /// <summary>
        /// Enumerates the directories that exist in this directory.
        /// </summary>
        /// <returns>A lazily-enumerated list of directories in the directory.</returns>
        public IEnumerable<AnyDirectoryPath> EnumerateDirectories()
        {
            foreach (var dir in Directory.EnumerateDirectories(RawPath))
            {
                yield return new AnyDirectoryPath(dir);
            }
        }
    }
}
