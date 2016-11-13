using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class TimeTests
    {
        public static IEnumerable<Func<AnyPath, DateTime>[]> UtcTimeMethods()
        {
            var methods = new Func<AnyPath, DateTime>[]
                {
                    _ => _.GetCreationTimeUtc(),
                    _ => _.GetLastAccessTimeUtc(),
                    _ => _.GetLastWriteTimeUtc(),
                };

            foreach (var method in methods)
            {
                yield return new Func<AnyPath, DateTime>[] { method };
            }
        }

        [Theory]
        [MemberData("UtcTimeMethods")]
        public void UtcTimesHaveUtcKind(Func<AnyPath, DateTime> method)
        {
            using (var file = new TemporaryFile())
            {
                var date = method(file.File);

                Assert.Equal(DateTimeKind.Utc, date.Kind);
            }
        }
        
        public static IEnumerable<Func<AnyPath, DateTime>[]> TimeMethods()
        {
            var methods = new Func<AnyPath, DateTime>[]
                {
                    _ => _.GetCreationTimeLocal(),
                    _ => _.GetLastAccessTimeLocal(),
                    _ => _.GetLastWriteTimeLocal(),
                };
            
            foreach (var method in methods)
            {
                yield return new Func<AnyPath, DateTime>[] { method };
            }
        }

        [Theory]
        [MemberData("TimeMethods")]
        public void LocalTimesHaveLocalKind(Func<AnyPath, DateTime> method)
        {
            using (var file = new TemporaryFile())
            {
                var date = method(file.File);

                Assert.Equal(DateTimeKind.Local, date.Kind);
            }
        }
    }
}
