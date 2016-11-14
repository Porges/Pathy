using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Pathy
{
    /// <summary>
    /// Represents an absolute or relative path to a file.
    /// </summary>
    public class AnyFilePath : AnyPath
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            // no extra invariants, yet
            Debug.Assert(this != null);
        }
        
        internal AnyFilePath(string filePath)
            : base(filePath)
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="AnyFilePath"/> from a base directory path and a relative file path.
        /// </summary>
        /// <param name="basePath">The base directory path.</param>
        /// <param name="relativePath">The relative file path.</param>
        public AnyFilePath(AnyDirectoryPath basePath, RelativeFilePath relativePath)
            : base(Combined(basePath, relativePath))
        {
            Invariant();
        }

        /// <summary>
        /// Creates a <see cref="AnyFilePath"/> from the given raw path.
        /// </summary>
        /// <param name="filePath">The path.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="filePath"/> was <c>null</c>.</exception>
        /// <exception cref="ArgumentException">There was a problem with the given path.</exception>
        /// <returns>A new <see cref="AnyFilePath"/> instance.</returns>
        public static new AnyFilePath From(string filePath)
        {
            Validation.CheckPath(filePath, nameof(filePath), Validations.IsFile);

            return new AnyFilePath(filePath);
        }

        private static string Combined(AnyDirectoryPath basePath, RelativeFilePath relativePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException(nameof(basePath));
            }

            if (relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            var result = Path.Combine(basePath.ToString(), relativePath.RawPath);

            // if we have an absolute path we normalize it,
            // so we can have consistent comparison
            return basePath.IsAbsolute ? Path.GetFullPath(result) : result;
        }

        /// <summary>
        /// Gets the extension of the file.
        /// </summary>
        /// <remarks>If no extension is present, this will return an empty string.</remarks>
        public string Extension => Path.GetExtension(RawPath);

        /// <summary>
        /// Creates a new file path with the given extension.
        /// </summary>
        /// <param name="extension">The new extension (with or without a leading '.').</param>
        /// <remarks>Passing <c>null</c> will remove the extension.</remarks>
        /// <returns>The new file path.</returns>
        public AnyFilePath WithExtension(string extension) =>
            new AnyFilePath(Path.ChangeExtension(RawPath, extension));

        /// <summary>
        /// Creates a new file path without any extension.
        /// </summary>
        /// <returns>The new file path.</returns>
        public AnyFilePath WithoutExtension() =>
            new AnyFilePath(Path.ChangeExtension(RawPath, null));

        /// <summary>
        /// Gets the name of the file represented by this <see cref="FilePath"/>.
        /// </summary>
        public FileName FileName =>
            new FileName(Path.GetFileName(RawPath));

        /// <summary>
        /// Creates a hard link to the file represented by this path.
        /// </summary>
        /// <param name="filePath">Where to create the hard link.</param>
        /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is <c>null</c>.</exception>
        public void CreateHardLinkAs(AnyFilePath filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            SafeNativeMethods.CreateHardLinkChecked(filePath.ToString(), RawPath);
        }

        /// <summary>
        /// Deletes the file represented by this path.
        /// </summary>
        public void Delete()
        {
            File.Delete(RawPath);
        }
        
        /// <summary>
        /// Sets the contents of the file to a string,
        /// using the default Unicode encoding.
        /// </summary>
        /// <param name="contents">The contents to replace the file with.</param>
        public void SetContents(string contents) =>
            File.WriteAllText(RawPath, contents);

        /// <summary>
        /// Sets the contents of the file to a string,
        /// with the specified encoding.
        /// </summary>
        /// <param name="contents">The contents to replace the file with.</param>
        /// <param name="encoding">The encoding to use.</param>
        public void SetContents(string contents, Encoding encoding) =>
            File.WriteAllText(RawPath, contents, encoding);

        /// <summary>
        /// Sets the contents of the file.
        /// </summary>
        /// <param name="contents">The contents to replace the file with.</param>
        public void SetContents(byte[] contents) =>
            File.WriteAllBytes(RawPath, contents);

        /// <summary>
        /// Retrieves the contents of the file as a string,
        /// automatically detecting the (Unicode) encoding.
        /// </summary>
        /// <returns>The contents as a string/</returns>
        public string GetContentsAsString() =>
            File.ReadAllText(RawPath);

        /// <summary>
        /// Retrieves the contents of the file as a string,
        /// using the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding of the file contents.</param>
        /// <returns>The contents as a string/</returns>
        public string GetContentsAsString(Encoding encoding) =>
            File.ReadAllText(RawPath, encoding);
        
        /// <summary>
        /// Retrieves the contents of the file as a byte array.
        /// </summary>
        /// <returns>The contents as a byte array.</returns>
        public byte[] GetContentsAsBytes() =>
            File.ReadAllBytes(RawPath);

        /// <summary>
        /// Updates the last-write time of the file that this path
        /// points to, or creates the file if it does not exist.
        /// </summary>
        public void Touch()
        {
            try
            {
                File.SetLastWriteTime(RawPath, DateTime.Now);
            }
            catch (FileNotFoundException)
            {
                // we use OpenOrCreate to avoid a race condition that the
                // file is created in the mean time
                File.Open(RawPath, FileMode.OpenOrCreate).Dispose();
            }
        }
    }
}
