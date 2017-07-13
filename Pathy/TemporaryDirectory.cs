using System;

namespace Pathy
{
    /// <summary>
    /// Represents a directory in the temporary directory that will
    /// be automatically deleted (including its contents) when
    /// this class is disposed.
    /// </summary>
    public sealed class TemporaryDirectory : IDisposable
    {
        /// <summary>
        /// The path of the temporary directory.
        /// </summary>
        public DirectoryPath Directory { get; } = DirectoryPath.CreateTemporary();

        /// <summary>
        /// Deletes the temporary directory (including its contents).
        /// </summary>
        public void Dispose() => Directory.Delete();
    }
}
