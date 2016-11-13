using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class Sorting
    {
        private static readonly AnyPath One = AnyPath.From("1");
        private static readonly AnyPath Two = AnyPath.From("2");
        private static readonly AnyPath Ten = AnyPath.From("10");
        private static readonly AnyPath Twenty = AnyPath.From("20");

        private static readonly IReadOnlyList<AnyPath> Input =
            new AnyPath[]
            {
                Twenty, Two, One, Ten
            };

        [Fact]
        public void LogicalSortHasDesiredOutcome()
        {
            var items = Input.ToList();
            items.Sort(PathComparer.Logical<AnyPath>());

            var expected = new[]
                {
                    One, Two, Ten, Twenty
                };

            Assert.Equal(expected, items);
        }

        [Fact]
        public void SimpleSortHasExpectedOutcome()
        {
            var items = Input.ToList();
            items.Sort(PathComparer.Default<AnyPath>());

            var expected = new[]
                {
                    One, Ten, Two, Twenty
                };

            Assert.Equal(expected, items);
        }
    }
}
