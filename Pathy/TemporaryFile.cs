using System;

namespace Pathy
{
    /// <summary>
    /// Represents a file in the temporary directory that will
    /// be automatically deleted when this class is disposed.
    /// </summary>
    public sealed class TemporaryFile : IDisposable
    {
        /// <summary>
        /// The path to the temporary file.
        /// </summary>
        public FilePath File { get; } = FilePath.CreateTemporary();

        /// <summary>
        /// Deletes the temporary file.
        /// </summary>
        public void Dispose() => File.Delete();
    }
}
