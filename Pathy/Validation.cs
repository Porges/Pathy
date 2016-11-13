using System;
using System.IO;

namespace Pathy
{
    [Flags]
    internal enum Validations
    {
        Default = 0,
        IsFileName = 1 << 0,
        Rooted = 1 << 1,
        NotRooted = 1 << 2,
        IsFile = 1 << 3,
    }

    internal static class Validation
    {
        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        public static void CheckPath(string path, string argumentName, Validations validations)
        {
            if (path == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            var badChars =
                (validations & Validations.IsFileName) != 0
                ? InvalidFileNameChars
                : InvalidPathChars;

            var badIndex = path.IndexOfAny(badChars);
            if (badIndex >= 0)
            {
                throw new InvalidPathException(path[badIndex], argumentName);
            }

            if ((validations & Validations.NotRooted) != 0)
            {
                if (Path.IsPathRooted(path))
                {
                    throw new ArgumentException("Path is absolute", argumentName);
                }
            }

            if ((validations & Validations.Rooted) != 0)
            {
                if (!Path.IsPathRooted(path))
                {
                    throw new ArgumentException("Path is relative", argumentName);
                }
            }

            if ((validations & Validations.IsFile) != 0)
            {
                // look for trailing ':', '\', '\.', '\..'
                for (int i = 0; i < 3; ++i)
                {
                    var ix = path.Length - (i + 1);
                    if (ix < 0)
                    {
                        throw new ArgumentException("File path expected", argumentName);
                    }

                    var c = path[ix];
                    if (c == '/' || c == '\\' || c == ':')
                    {
                        throw new ArgumentException("File path expected", argumentName);
                    }
                    
                    if (c != '.')
                    {
                        break;
                    }
                }

                if (Path.GetPathRoot(path) == path.Replace('/', '\\'))
                {
                    throw new ArgumentException("File path expected", argumentName);
                }
            }
        }        
    }
}
