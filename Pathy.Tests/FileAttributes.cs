using System.IO;
using Xunit;

namespace Pathy.Tests
{
    public class FileAttributesTests
    {
        [Fact]
        public void TempDirectoryIsDirectory()
        {
            using (var dir = new TemporaryDirectory())
            {
                Assert.True(dir.Directory.GetAttributes().HasFlag(FileAttributes.Directory));
            }
        }

        [Fact]
        public void TempFileIsFile()
        {
            using (var file = new TemporaryFile())
            {
                Assert.False(file.File.GetAttributes().HasFlag(FileAttributes.Directory));
            }
        }
    }
}
