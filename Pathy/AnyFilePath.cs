using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Pathy
{
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
        
        public AnyFilePath(AnyDirectoryPath basePath, RelativeFilePath relativePath)
            : base(Combined(basePath, relativePath))
        {
            Invariant();
        }

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
        
        public AnyFilePath WithExtension(string extension) =>
            new AnyFilePath(Path.ChangeExtension(RawPath, extension));

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
        
        public void SetContents(string contents) =>
            File.WriteAllText(RawPath, contents);

        public void SetContents(string contents, Encoding encoding) =>
            File.WriteAllText(RawPath, contents, encoding);

        public void SetContents(byte[] contents) =>
            File.WriteAllBytes(RawPath, contents);

        public string GetContentsAsString() =>
            File.ReadAllText(RawPath);

        public string GetContentsAsString(Encoding encoding) =>
            File.ReadAllText(RawPath, encoding);

        public byte[] GetContentsAsBytes() =>
            File.ReadAllBytes(RawPath);

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
