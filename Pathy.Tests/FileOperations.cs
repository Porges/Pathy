using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class FileOperations
    {
        [Fact]
        public void TouchingFileUpdatesItsWriteTime()
        {
            using (var dir = new TemporaryDirectory())
            {
                var file = dir.Directory / FileName.From("touched.txt");

                file.Touch();
                var lastWriteTime = file.GetLastWriteTimeUtc();

                Thread.Sleep(1); // :(

                file.Touch();
                var newLastWriteTime = file.GetLastWriteTimeUtc();

                Assert.True(newLastWriteTime > lastWriteTime);
            }
        }

        [Fact]
        public void CreatingSameHardlinkTwiceFails()
        {
            using (var dir = new TemporaryDirectory())
            {
                var source = dir.Directory / FileName.From("source");
                source.Touch();

                var link = source.WithExtension(".link");
                source.CreateHardLinkAs(link);

                Assert.Throws<IOException>(() => source.CreateHardLinkAs(link));
            }
        }

        [Fact]
        public void CanChangeHardLinkSourceAndHaveLinkUpdate()
        {
            const string Original = "original content";
            const string Changed = "changed content";

            using (var tmp = new TemporaryFile())
            {
                var source = tmp.File;
                var link = source.WithExtension(".link");

                source.SetContents(Original);
                source.CreateHardLinkAs(link);
                try
                {

                    Assert.Equal(Original, link.GetContentsAsString());

                    source.SetContents(Changed);

                    Assert.Equal(Changed, link.GetContentsAsString());
                }
                finally
                {
                    link.Delete();
                }
            }
        }
    }
}
