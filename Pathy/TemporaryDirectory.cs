using System;

namespace Pathy
{
    public sealed class TemporaryDirectory : IDisposable
    {
        public DirectoryPath Directory { get; } = DirectoryPath.CreateTemporary();

        public void Dispose()
        {
            Directory.Delete();
        }
    }
}
