using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class Formatting
    {
        private static readonly AnyPath Path = AnyPath.From(@"C:\path\to\a\file.txt");

        [Fact]
        public void CanFormatPlainPath()
        {
            Assert.Equal(Path.ToString(), $"{Path}");
        }

        [Fact]
        public void CanFormatToMoreWidth()
        {
            Assert.Equal(Path.ToString(), $"{Path:100}");
        }

        [Fact]
        public void CanFormatToExactSize()
        {
            Assert.Equal(Path.ToString(), $"{Path:21}");
        }

        [Fact]
        public void CanTruncatePathToJustFileName()
        {
            Assert.Equal(@"...\file.txt", $"{Path:12}");
        }

        [Fact]
        public void CanTruncatePathToOneCharShorter()
        {
            Assert.Equal(@"C:\path\...\file.txt", $"{Path:20}");
        }

        [Fact]
        public void AWidthOfZeroThrows()
        {
            Assert.Throws<FormatException>(() => $"{Path:0}");
        }
        
        [Fact]
        public void NegativeWidthThrows() // TODO: should this right-align?
        {
            Assert.Throws<FormatException>(() => $"{Path:-10}");
        }

        [Fact]
        public void CanHaveSpacesInFormat()
        {
            Assert.Equal(Path.ToString(), $"{Path: 22}");
        }

        [Fact]
        public void GreaterThanMaxIntThrows()
        {
            Assert.Throws<FormatException>(() => $"{Path:2147483648}");
        }

        [Fact]
        public void MaxIntThrows()
        {
            Assert.Throws<FormatException>(() => $"{Path:2147483647}");
        }

        [Fact]
        public void OneLessThanMaxIntIsOkay()
        {
            // this is okay because we don't bother formatting if the size is bigger
            Assert.Equal(Path.ToString(), $"{Path:2147483646}");
        }
        
        [Fact]
        public void FormattingAMaxLengthPathIsOkay()
        {
            // MAX_PATH is 260
            var twoSixtyChars = AnyPath.From(new string('x', 260));
            Assert.Equal(twoSixtyChars.ToString(), $"{twoSixtyChars:260}");
        }

        [Fact]
        public void TruncatingToMaxPathLengthIsOkay()
        {
            // MAX_PATH is 260
            var twoSixtyChars = AnyPath.From(new string('x', 261));

            Assert.Equal(260, $"{twoSixtyChars:260}".Length);
        }
        
        [Fact]
        public void TruncatingAReallyLongPathToAReallyLongStringIsOkay()
        {
            // wooh, .NET 4.6.2

            var twoSixtyChars = AnyPath.From(new string('x', 3200));
            Assert.Equal(1234, $"{twoSixtyChars:1234}".Length);
        }


    }
}
