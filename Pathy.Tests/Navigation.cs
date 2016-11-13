using System.IO;
using Xunit;

namespace Pathy.Tests
{
    public class Navigation
    {
        [Theory]
        [InlineData(@"C:\data", @"C:\")]
        [InlineData(@"C:\data\test.txt", @"C:\data")]
        public void ParentTests(string given, string expected)
        {
            var path = FilePath.From(given);
            Assert.Equal(expected, path.Directory.ToString());
        }

        [Fact]
        public void CurrentDirIsAbsolute()
        {
            var path = DirectoryPath.Current();
            Assert.Equal(Path.IsPathRooted(path.ToString()), path.IsAbsolute);
        }

        [Fact]
        public void CanExtractFileName()
        {
            var pwd = DirectoryPath.Current();
            var fileName = FileName.From("some_random_file.txt");

            var composed = pwd / fileName;
            var extracted = composed.FileName;

            Assert.Equal(fileName, extracted);
        }
    }
}
