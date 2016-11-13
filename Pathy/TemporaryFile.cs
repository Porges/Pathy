using System;

namespace Pathy
{
    public sealed class TemporaryFile : IDisposable
    {
        public FilePath File { get; } = FilePath.CreateTemporary();

        public void Dispose()
        {
            File.Delete();
        }
    }
}
