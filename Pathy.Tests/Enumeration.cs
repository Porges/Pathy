using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class Enumeration
    {
        [Fact]
        public void EnumeratingFilesOnlyReturnsFiles()
        {
            using (var tmp = new TemporaryDirectory())
            {
                var file = tmp.Directory / FileName.From("hello.txt");
                var dir = tmp.Directory / RelativeDirectoryPath.From("dir");

                file.Touch();
                dir.Create();

                var found = Assert.Single(tmp.Directory.EnumerateFiles());
                Assert.Equal(file, found);
            }
        }
        
        [Fact]
        public void EnumeratingDirectoriesOnlyReturnsDirectories()
        {
            using (var tmp = new TemporaryDirectory())
            {
                var file = tmp.Directory / FileName.From("hello.txt");
                var dir = tmp.Directory / RelativeDirectoryPath.From("dir");

                file.Touch();
                dir.Create();

                var found = Assert.Single(tmp.Directory.EnumerateDirectories());
                Assert.Equal(dir, found);
            }
        }

        [Fact]
        public void CanEnumerateFilesInRelativeDirectory()
        {
            using (var tmp = new TemporaryDirectory())
            {

            }
        }
    }
}
