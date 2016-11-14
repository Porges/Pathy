using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class UniformFileApi
    {
        public static object[][] Instances =
            {
                new object[] { AnyFilePath.From("file.ext") },
                new object[] { FilePath.From(@"C:\file.ext") },
                new object[] { RelativeFilePath.From(@"path\to\file.ext") },
                new object[] { FileName.From("file.ext") },
            };

        [Theory]
        [MemberData(nameof(Instances))]
        public void CanChangeExtension(dynamic file)
        {
            var ext = ".new";
            var changed = file.WithExtension(ext);

            Assert.IsType(file.GetType(), changed);
            Assert.Equal(ext, changed.Extension);
        }

        [Theory]
        [MemberData(nameof(Instances))]
        public void CanChangeExtensionWithoutDot(dynamic file)
        {
            var ext = "new";
            var changed = file.WithExtension(ext);

            Assert.IsType(file.GetType(), changed);
            Assert.Equal("." + ext, changed.Extension);
        }


        [Theory]
        [MemberData(nameof(Instances))]
        public void CanRemoveExtension(dynamic file)
        {
            var changed = file.WithoutExtension();

            Assert.IsType(file.GetType(), changed);
            Assert.Equal("", changed.Extension);
        }
    }
}
