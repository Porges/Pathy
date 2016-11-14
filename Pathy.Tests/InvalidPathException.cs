using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class Exceptions
    {
        private static T RoundTrip<T>(T it)
        {
            var formatter = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, it);
                ms.Seek(0, SeekOrigin.Begin);

                return (T)formatter.Deserialize(ms);
            }
        }

        public static IEnumerable<object[]> Instances()
        {
            // one instance per constructor
            var instances = new[]
                {
                    new InvalidPathException(),
                    new InvalidPathException("custom message"),
                    new InvalidPathException('x', "paramName"),
                    new InvalidPathException("custom message", new Exception("inner")),
                };

            foreach (var instance in instances)
            {
                yield return new object[] { instance };
            }
        }

        [Theory]
        [MemberData("Instances")]
        public void CanRoundTrip(InvalidPathException ex)
        {
            var roundTripped = RoundTrip(ex);

            Assert.Equal(ex.Message, roundTripped.Message);
            Assert.Equal(ex.InnerException?.Message, roundTripped.InnerException?.Message);
        }

        [Fact]
        public void MessageContainsCharacterDetails()
        {
            var ex = Assert.Throws<InvalidPathException>(() => AnyPath.From("|"));
            Assert.Contains("\'|\'", ex.Message);
            Assert.Contains("(U+007C)", ex.Message);
        }
    }
}
