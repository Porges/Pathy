using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Pathy.Tests
{
    public class UniformApi
    {
        private static IEnumerable<string> GetExemplarStrings(Type type)
        {
            // we group files and directories together here,
            // because there's no syntactic difference:

            if (type == typeof(FilePath) ||
                type == typeof(DirectoryPath) ||
                type == typeof(AnyFilePath) ||
                type == typeof(AnyDirectoryPath) ||
                type == typeof(AnyPath))
            {
                // file look-a-likes:
                yield return @"C:\path\to\file.txt";
                yield return @"\\computer\path\to\file.txt";
                yield return @"C:\file.txt";
                yield return @"\\computer\file.txt";

                // dir look-a-likes:
                yield return @"C:\path\to\dir";
                yield return @"\\computer\path\to\dir";
            }

            if (type == typeof(DirectoryPath) ||
               type == typeof(AnyDirectoryPath) ||
               type == typeof(AnyPath))
            {
                yield return @"C:\path\to\dir\";
                yield return @"\\computer\path\to\dir\";

                //yield return @"\\computer";
                //yield return @"\\computer\";

                yield return @"C:\";
                yield return @"C:";
            }

            if (type == typeof(RelativeFilePath) ||
                type == typeof(RelativeDirectoryPath) ||
                type == typeof(AnyFilePath) ||
                type == typeof(AnyDirectoryPath) ||
                type == typeof(AnyPath))
            {
                // file look-a-likes:
                yield return @"path\to\file.txt";
                yield return @"file.txt";
                yield return @".\path\to\file.txt";
                yield return @".\file.txt";

                // dir look-a-likes:
                yield return @"dir";
                yield return @"path\to\dir";
                yield return @".\dir";
                yield return @".\path\to\dir";
            }


            if (type == typeof(RelativeDirectoryPath) ||
                type == typeof(AnyDirectoryPath) ||
                type == typeof(AnyPath))
            {
                yield return @"dir\";
                yield return @"path\to\dir\";
                yield return @".\dir\";
                yield return @".\path\to\dir\";
            }
        }

        private static IEnumerable<dynamic> GetExemplars(Type type)
        {
            var from = GetFromMethod(type);
            var args = new object[1];
            foreach (var path in GetExemplarStrings(type))
            {
                args[0] = path;
                yield return from.Invoke(null, args);
            }
        }

        private static MethodInfo GetFromMethod(Type type) => type.GetMethod("From");

        public static readonly object[] Types =
            {
                new [] { typeof(AnyDirectoryPath) },
                new [] { typeof(AnyFilePath) },
                new [] { typeof(FilePath) },
                new [] { typeof(DirectoryPath) },
                new [] { typeof(RelativeDirectoryPath) },
                new [] { typeof(RelativeFilePath) },
                new [] { typeof(AnyPath) },
                new [] { typeof(FileName) },
            };

        public class RawConstructor
        {
            [Theory]
            [MemberData("Types", MemberType = typeof(UniformApi))]
            public void IsNotPublic(Type type)
            {
                var constructor = type.GetConstructor(new[] { typeof(string) });
                Assert.Null(constructor);
            }
        }

        public class FromMethod
        {

            [Theory]
            [MemberData("Types", MemberType = typeof(UniformApi))]
            public void ExistsAndHasCorrectType(Type type)
            {
                var method = GetFromMethod(type);
                Assert.NotNull(method);
                Assert.Equal(typeof(string), method.GetParameters().Single().ParameterType);
                Assert.Equal(type, method.ReturnType);
            }

            [Theory]
            [MemberData("Types", MemberType = typeof(UniformApi))]
            public void ThrowsOnNullArgument(Type type)
            {
                var method = GetFromMethod(type);
                var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { null }));
                var innerEx = Assert.IsType<ArgumentNullException>(ex.InnerException);

                Assert.Equal(method.GetParameters().Single().Name, innerEx.ParamName);
            }


            [Theory]
            [MemberData("Types", MemberType = typeof(UniformApi))]
            public void ThrowsIfPathContainsInvalidCharacter(Type type)
            {
                var method = GetFromMethod(type);

                foreach (var c in Path.GetInvalidPathChars())
                {
                    var ex = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { c.ToString() }));
                    var innerEx = Assert.IsType<InvalidPathException>(ex.InnerException);

                    Assert.Equal(method.GetParameters().Single().Name, innerEx.ParamName);
                }
            }

            [Theory]
            [MemberData("Types", MemberType = typeof(UniformApi))]
            public void InvariantsExistInDebugModeAndNotInRelease(Type type)
            {
                var invariantMethod = type.GetMethod("Invariant", BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance);

#if DEBUG
                Assert.NotNull(invariantMethod);
#else
                Assert.Null(invariantMethod);
#endif
            }
        }

        public class SlashOperator
        {
            public static object[] AllowedCombinations =
                {
                    new[] { typeof(DirectoryPath), typeof(RelativeDirectoryPath), typeof(DirectoryPath) },
                    new[] { typeof(DirectoryPath), typeof(RelativeFilePath), typeof(FilePath) },
                    new[] { typeof(RelativeDirectoryPath), typeof(RelativeDirectoryPath), typeof(RelativeDirectoryPath) },
                    new[] { typeof(RelativeDirectoryPath), typeof(RelativeFilePath), typeof(RelativeFilePath) },
                    new[] { typeof(AnyDirectoryPath), typeof(RelativeDirectoryPath), typeof(AnyDirectoryPath) },
                    new[] { typeof(AnyDirectoryPath), typeof(RelativeFilePath), typeof(AnyFilePath) },
                };

            [Theory]
            [MemberData("AllowedCombinations")]
            public void WorksForExpectedCombinations(Type leftType, Type rightType, Type resultType)
            {
                foreach (var l in GetExemplars(leftType))
                foreach (var r in GetExemplars(rightType))
                {
                    var result = l / r;
                    Assert.IsType(resultType, result);
                }
            }
        }
    }
}
