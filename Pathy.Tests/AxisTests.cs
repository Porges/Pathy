using System;
using Xunit;

namespace Pathy.Tests
{
    public class AxisTests
    {
        public class Directory
        {
            private const string NonRooted = @"relative\path";
            private const string Rooted = @"C:\relative\path";
            private const string UncRooted = @"\\computer\relative\path";
            private const string ParamName = "directoryPath";

            public class Absolute
            {
                [Fact]
                public void ThrowsOnNonRootedPaths()
                {
                    var ex = Assert.Throws<ArgumentException>(() => DirectoryPath.From(NonRooted));
                    Assert.Equal(ParamName, ex.ParamName);
                }

                [Fact]
                public void AcceptsUncPaths()
                {
                    var path = DirectoryPath.From(UncRooted);
                    Assert.True(path.IsAbsolute);
                }

                [Fact]
                public void AcceptsRootedPaths()
                {
                    var path = DirectoryPath.From(Rooted);
                    Assert.True(path.IsAbsolute);
                }
            }

            public class Relative
            {
                [Fact]
                public void AcceptsNonRootedPaths()
                {
                    var path = RelativeDirectoryPath.From(NonRooted);
                    Assert.True(path.IsRelative);
                }

                [Fact]
                public void ThrowsOnUncPaths()
                {
                    var ex = Assert.Throws<ArgumentException>(() => RelativeDirectoryPath.From(UncRooted));
                    Assert.Equal(ParamName, ex.ParamName);
                }

                [Fact]
                public void ThrowsOnRootedPaths()
                {
                    var ex = Assert.Throws<ArgumentException>(() => RelativeDirectoryPath.From(Rooted));
                    Assert.Equal(ParamName, ex.ParamName);
                }
            }
        }

        public class File
        {
            private const string Rooted = @"C:\file.txt";
            private const string NonRooted = @"relative\file.txt";
            private const string UncRooted = @"\\computer\C\file.txt";
            private const string FileOnly = "file.txt";

            private const string ParamName = "filePath";
            private const string FileNameParamName = "fileName";
            

            public static readonly object[][] DirectoryPaths =
                {
                    new object[] { @"dir\" },
                    new object[] { @"C:\path\to\dir\" },
                    new object[] { @"\\computer\path\to\dir\" },
                    new object[] { @"\\computer" },
                    new object[] { @"\\computer\" },
                    new object[] { @"\\computer\C" },
                    new object[] { @"\\computer\C\" },
                    new object[] { @"C:\" },
                    new object[] { @"C:" },
                    new object[] { @"C:\path\to\.." },
                    new object[] { @"C:\path\to\." },

                    new object[] { @"dir/" },
                    new object[] { @"C:/path/to/dir/" },
                    new object[] { @"\\computer/path/to/dir/" },
                    new object[] { @"\\computer" },
                    new object[] { @"\\computer/" },
                    new object[] { @"\\computer/C" },
                    new object[] { @"\\computer/C/" },
                    new object[] { @"C:/" },
                    new object[] { @"C:" },
                    new object[] { @"C:/path/to/.." },
                    new object[] { @"C:/path/to/." },
                };

            public class Absolute
            {
                [Fact]
                public void ThrowsOnNonRootedPaths()
                {
                    var ex = Assert.Throws<ArgumentException>(() => FilePath.From(NonRooted));
                    Assert.Equal(ParamName, ex.ParamName);
                }

                [Fact]
                public void AcceptsUncPaths()
                {
                    var path = FilePath.From(UncRooted);
                    Assert.True(path.IsAbsolute);
                }

                [Fact]
                public void AcceptsRootedPaths()
                {
                    var path = FilePath.From(Rooted);
                    Assert.True(path.IsAbsolute);
                }

                [Theory]
                [MemberData("DirectoryPaths", MemberType = typeof(File))]
                public void DirectoriesAreNotFiles(string directory)
                {
                    Assert.Throws<ArgumentException>(() => FilePath.From(directory));
                }
            }

            public class Relative
            {
                [Fact]
                public void AcceptsNonRootedPaths()
                {
                    var path = RelativeFilePath.From(NonRooted);
                    Assert.True(path.IsRelative);
                }

                [Fact]
                public void ThrowsOnUncPaths()
                {
                    var ex = Assert.Throws<ArgumentException>(() => RelativeFilePath.From(UncRooted));
                    Assert.Equal(ParamName, ex.ParamName);
                }

                [Fact]
                public void ThrowsOnRootedPaths()
                {
                    var ex = Assert.Throws<ArgumentException>(() => RelativeFilePath.From(Rooted));
                    Assert.Equal(ParamName, ex.ParamName);
                }

                [Theory]
                [MemberData("DirectoryPaths", MemberType = typeof(File))]
                public void DirectoriesAreNotFiles(string directory)
                {
                    Assert.Throws<ArgumentException>(() => RelativeFilePath.From(directory));
                }
            }


            public class JustFileName
            {
                [Fact]
                public void AcceptsFileNames()
                {
                    var path = FileName.From(FileOnly);
                    Assert.True(path.IsRelative);
                }

                [Fact]
                public void ThrowsOnNonRootedPaths()
                {
                    var ex = Assert.Throws<InvalidPathException>(() => FileName.From(NonRooted));
                    Assert.Equal(FileNameParamName, ex.ParamName);
                }

                [Fact]
                public void ThrowsOnUncPaths()
                {
                    var ex = Assert.Throws<InvalidPathException>(() => FileName.From(UncRooted));
                    Assert.Equal(FileNameParamName, ex.ParamName);
                }

                [Fact]
                public void ThrowsOnRootedPaths()
                {
                    var ex = Assert.Throws<InvalidPathException>(() => FileName.From(Rooted));
                    Assert.Equal(FileNameParamName, ex.ParamName);
                }

                [Theory]
                [MemberData("DirectoryPaths", MemberType = typeof(File))]
                public void DirectoriesAreNotFiles(string directory)
                {
                    var ex = Assert.Throws<InvalidPathException>(() => FileName.From(directory));
                    Assert.Equal(FileNameParamName, ex.ParamName);
                }
            }
        }
    }
}
