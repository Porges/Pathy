using System;
using System.Diagnostics;
using System.IO;

namespace Pathy
{
    /// <summary>
    /// Represents a filename.
    /// </summary>
    /// <remarks>This is represented as a relative file path with no directory part.</remarks>
    public class FileName : RelativeFilePath
    {
        [Conditional(BuildType.Debug)]
        private void Invariant()
        {
            Debug.Assert(RawPath.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
        }

        internal FileName(string fileName)
            : base(fileName)
        {
            Invariant();
        }

        public new static FileName From(string fileName)
        {
            Validation.CheckPath(fileName, nameof(fileName), Validations.IsFileName);

            return new FileName(fileName);
        }

        public new FileName WithExtension(string extension) =>
            new FileName(Path.ChangeExtension(RawPath, extension));

        public new FileName WithoutExtension() =>
            new FileName(Path.ChangeExtension(RawPath, null));
    }
}
