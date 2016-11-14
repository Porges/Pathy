using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pathy.Tests
{
    public class NullArgument
    {
        public static void Check(Expression<Action> expr)
        {
            var ex = Assert.Throws<ArgumentNullException>(expr.Compile());

            var call = expr.Body as MethodCallExpression;
            if (call != null)
            {
                var args = call.Arguments;
                var parameters = call.Method.GetParameters();

                DoAssertion(ex, args, parameters);
                return;
            }

            var constructor = expr.Body as NewExpression;
            if (constructor != null)
            {
                var args = constructor.Arguments;
                var parameters = constructor.Constructor.GetParameters();

                DoAssertion(ex, args, parameters);
                return;
            }

            Assert.False(true, "Unsupported");
        }

        private static void DoAssertion(ArgumentNullException ex, ReadOnlyCollection<Expression> args, ParameterInfo[] parameters)
        {
            int ix = 0;
            foreach (var arg in args)
            {
                var constant = arg as ConstantExpression;
                if (constant != null && constant.Value == null)
                {
                    break;
                }

                ++ix;
            }

            Assert.True(ix < args.Count, "Unable to find null argument");
            Assert.Equal(ex.ParamName, parameters[ix].Name);
        }

        [Fact]
        public void CreateHardLinkValidatesArgument()
        {
            var path = FilePath.From(@"C:\file.txt");
            Check(() => path.CreateHardLinkAs(null));
        }

        public class Directory
        {
            public class Relative
            {
                [Fact]
                public void BaseArgument()
                {
                    var relative = RelativeDirectoryPath.From("dir");
                    Check(() => new RelativeDirectoryPath(null, relative));
                }

                [Fact]
                public void RelativeArgument()
                {
                    var relative = RelativeDirectoryPath.From("dir");
                    Check(() => new RelativeDirectoryPath(relative, null));
                }
            }

            public class Absolute
            {
                [Fact]
                public void BaseArgument()
                {
                    var relative = RelativeDirectoryPath.From("dir");
                    Check(() => new DirectoryPath(null, relative));
                }

                [Fact]
                public void RelativeArgument()
                {
                    var relative = DirectoryPath.From(@"C:\");
                    Check(() => new DirectoryPath(relative, null));
                }
            }
        }

        public class File
        {
            public class Relative
            {
                [Fact]
                public void BaseArgument()
                {
                    var relative = RelativeFilePath.From("file");
                    Check(() => new RelativeFilePath(null, relative));
                }

                [Fact]
                public void RelativeArgument()
                {
                    var relative = RelativeDirectoryPath.From("dir");
                    Check(() => new RelativeFilePath(relative, null));
                }
            }

            public class Absolute
            {
                [Fact]
                public void BaseArgument()
                {
                    var relative = RelativeFilePath.From("file");
                    Check(() => new FilePath(null, relative));
                }

                [Fact]
                public void RelativeArgument()
                {
                    var relative = DirectoryPath.From(@"C:\");
                    Check(() => new FilePath(relative, null));
                }
            }
        }
    }
}
