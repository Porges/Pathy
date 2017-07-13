using Xunit;

namespace Pathy.Tests
{
    public class BuilderTests
    {
        [Fact]
        public void TemporaryDirectoryDeletesContentsOnDisposal()
        {
            var dirBuilder =
                new Builder.Directory
                {
                    ["hello.txt"] = "hello, world!",
                    ["dir"] =
                        new Builder.Directory
                        {
                            ["nested.txt"] = "I am nested!"
                        }
                };

            using (var tmpDir = dirBuilder.BuildTemporary())
            {
                var dir = tmpDir.Directory;

                Assert.Equal("hello, world!", (dir / "hello.txt").GetContentsAsString());
                Assert.Equal("I am nested!", (dir / "dir/nested.txt").GetContentsAsString());
                tmpDir.Dispose();

                Assert.False((dir/"hello.txt").Exists());
                Assert.False((dir/"dir/nested.txt").Exists());
            }
        }
    }
}
