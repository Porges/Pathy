using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class ContentsTests
    {
        [Fact]
        public void SettingContentDefaultsToUtf8()
        {
            using (var tmp = new TemporaryFile())
            {
                var message = "once upon a time";
                tmp.File.SetContents(message);

                Assert.Equal(Encoding.UTF8.GetBytes(message), tmp.File.GetContentsAsBytes());
            }
        }

        public static IEnumerable<Encoding[]> UnicodeEncodings()
        {
            var encodings = new Encoding[]
                {
                    new UTF8Encoding(true, true),
                    new UTF8Encoding(false, true), // no BOM (only possible with UTF-8)
                    new UnicodeEncoding(true, true), // UTF16BE
                    new UnicodeEncoding(false, true), // UTF16LE
                    new UTF32Encoding(true, true), // UTF32BE
                    new UTF32Encoding(false, true), // UTF32LE
                };

            foreach (var encoding in encodings)
            {
                yield return new[] { encoding };
            }
        }

        [Theory]
        [MemberData("UnicodeEncodings")]
        public void UnicodeEncodingsAreDetectedAutomatically(Encoding encoding)
        {
            using (var tmp = new TemporaryFile())
            {
                var message = "once upon a time";
                tmp.File.SetContents(message, encoding);

                Assert.Equal(message, tmp.File.GetContentsAsString());
            }
        }
    }
}
