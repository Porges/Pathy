using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class Comparisons
    { 
        public static object[][] Sets =
            {
                new[]
                {
                    new
                    {
                        Smaller = RelativeFilePath.From(@"path\to\a.txt"),
                        Base = RelativeFilePath.From(@"path\to\b.txt"),
                        Bigger = RelativeFilePath.From(@"path\to\c.txt"),
                        Same = RelativeFilePath.From(@"path\to\B.txt"),
                    },
                },
                new[]
                {
                    new
                    {
                        Smaller = FilePath.From(@"c:\path\to\a.txt"),
                        Base = FilePath.From(@"c:\path\to\b.txt"),
                        Bigger = FilePath.From(@"c:\path\to\c.txt"),
                        Same = FilePath.From(@"c:\path\to\B.txt"),
                    }
                },
                new[]
                {
                    new
                    {
                        Smaller = RelativeDirectoryPath.From(@"path\to\a"),
                        Base = RelativeDirectoryPath.From(@"path\to\b"),
                        Bigger = RelativeDirectoryPath.From(@"path\to\c"),
                        Same = RelativeDirectoryPath.From(@"path\to\B"),
                    },
                },
                new[]
                {
                    new
                    {
                        Smaller = DirectoryPath.From(@"c:\path\to\a"),
                        Base = DirectoryPath.From(@"c:\path\to\b"),
                        Bigger = DirectoryPath.From(@"c:\path\to\c"),
                        Same = DirectoryPath.From(@"c:\path\to\B"),
                    }
                },
                new[]
                {
                    new
                    {
                        Smaller = FileName.From(@"a"),
                        Base = FileName.From(@"b"),
                        Bigger = FileName.From(@"c"),
                        Same = FileName.From(@"B"),
                    }
                }
            };

        [Theory]
        [MemberData(nameof(Sets))]
        public void CheckLessThan(dynamic set)
        {
            Assert.True(set.Smaller < set.Base);
            Assert.True(set.Base < set.Bigger);
            Assert.True(set.Smaller < set.Bigger);

            Assert.False(set.Base < set.Smaller);
            Assert.False(set.Bigger < set.Base);
            Assert.False(set.Bigger < set.Smaller);

            Assert.False(set.Base < set.Same);
            Assert.False(set.Same < set.Base);

            Assert.True(null < set.Base);
            Assert.False(set.Base < null);
        }


        [Theory]
        [MemberData(nameof(Sets))]
        public void CheckGreaterThan(dynamic set)
        {
            Assert.True(set.Base > set.Smaller);
            Assert.True(set.Bigger > set.Base);
            Assert.True(set.Bigger > set.Smaller);
            
            Assert.False(set.Smaller > set.Base);
            Assert.False(set.Base > set.Bigger);
            Assert.False(set.Smaller > set.Bigger);

            Assert.False(set.Base > set.Same);
            Assert.False(set.Same > set.Base);

            Assert.True(set.Base > null);
            Assert.False(null > set.Base);
        }


        [Theory]
        [MemberData(nameof(Sets))]
        public void CheckLessThanOrEqualTo(dynamic set)
        {
            Assert.True(set.Smaller <= set.Base);
            Assert.True(set.Base <= set.Bigger);
            Assert.True(set.Smaller <= set.Bigger);
            
            Assert.False(set.Base <= set.Smaller);
            Assert.False(set.Bigger <= set.Base);
            Assert.False(set.Bigger <= set.Smaller);

            Assert.True(set.Base <= set.Same);
            Assert.True(set.Same <= set.Base);

            Assert.True(null <= set.Base);
            Assert.False(set.Base <= null);
        }

        [Theory]
        [MemberData(nameof(Sets))]
        public void CheckGreaterThanOrEqualTo(dynamic set)
        {
            Assert.True(set.Base >= set.Smaller);
            Assert.True(set.Bigger >= set.Base);
            Assert.True(set.Bigger >= set.Smaller);
            
            Assert.False(set.Smaller >= set.Base);
            Assert.False(set.Base >= set.Bigger);
            Assert.False(set.Smaller >= set.Bigger);

            Assert.True(set.Same >= set.Base);
            Assert.True(set.Base >= set.Same);

            Assert.True(set.Base >= null);
            Assert.False(null >= set.Base);
        }


        [Theory]
        [MemberData(nameof(Sets))]
        public void CheckEqualTo(dynamic set)
        {
            Assert.False(set.Base == set.Smaller);
            Assert.False(set.Bigger == set.Base);
            Assert.False(set.Bigger == set.Smaller);

            Assert.False(set.Smaller == set.Base);
            Assert.False(set.Base == set.Bigger);
            Assert.False(set.Smaller == set.Bigger);
            
            Assert.True(set.Same == set.Base);
            Assert.True(set.Base == set.Same);

            Assert.False(set.Base == null);
            Assert.False(null == set.Base);
        }


        [Theory]
        [MemberData(nameof(Sets))]
        public void NotEqualTo(dynamic set)
        {
            Assert.True(set.Base != set.Smaller);
            Assert.True(set.Bigger != set.Base);
            Assert.True(set.Bigger != set.Smaller);

            Assert.True(set.Smaller != set.Base);
            Assert.True(set.Base != set.Bigger);
            Assert.True(set.Smaller != set.Bigger);

            Assert.False(set.Same != set.Base);
            Assert.False(set.Base != set.Same);

            Assert.True(set.Base != null);
            Assert.True(null != set.Base);
        }

        [Theory]
        [MemberData(nameof(Sets))]
        public void EqualsMethod(dynamic set)
        {
            Assert.True(set.Base.Equals(set.Base));
            Assert.True(set.Base.Equals(set.Same));
            Assert.True(set.Same.Equals(set.Base));

            Assert.False(set.Base.Equals(null));

            Assert.False(set.Base.Equals(set.Smaller));
            Assert.False(set.Base.Equals(set.Bigger));
            Assert.False(set.Smaller.Equals(set.Base));
            Assert.False(set.Bigger.Equals(set.Base));
        }
        
        [Theory]
        [MemberData(nameof(Sets))]
        public void ObjectEquals(dynamic set)
        {
            Assert.True(set.Base.Equals((object)set.Base));
            Assert.True(set.Base.Equals((object)set.Same));
            Assert.True(set.Same.Equals((object)set.Base));

            Assert.False(set.Base.Equals((object)null));

            Assert.False(set.Base.Equals((object)set.Smaller));
            Assert.False(set.Base.Equals((object)set.Bigger));
            Assert.False(set.Smaller.Equals((object)set.Base));
            Assert.False(set.Bigger.Equals((object)set.Base));
        }

        [Theory]
        [MemberData(nameof(Sets))]
        public void GetHasHCode(dynamic set)
        {
            var baseHash = set.Base.GetHashCode();
            Assert.Equal(baseHash, set.Same.GetHashCode());
            Assert.NotEqual(baseHash, set.Smaller.GetHashCode());
            Assert.NotEqual(baseHash, set.Bigger.GetHashCode());
        }
    }
}
