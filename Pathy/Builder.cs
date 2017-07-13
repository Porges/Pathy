using Pathy.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pathy
{
    /// <summary>
    /// A builder creates a file or directory.
    /// </summary>
    public abstract class Builder
    {
        private Builder()
        {
        }

        /// <summary>
        /// Creates a <see cref="File"/> builder that creates a file with the given content,
        /// using the default (no-BOM UTF-8) encoding.
        /// </summary>
        /// <param name="fileContent">The content to put in the file.</param>
        public static implicit operator Builder(string fileContent)
                => new File(fileContent);

        /// <summary>
        /// Build the file or directory at the given path.
        /// </summary>
        /// <param name="path">The path to create at.</param>
        public abstract void Build(string path);
        
        /// <summary>
        /// Creates a file at the given path.
        /// </summary>
        public sealed class File : Builder
        {
            private readonly Encoding _encoding;
            private readonly string _fileContent;

            /// <summary>
            /// Creates a <see cref="File"/> builder that creates a file with the given content,
            /// using the default (no-BOM UTF-8) encoding.
            /// </summary>
            /// <param name="fileContent">The content to put in the file.</param>
            public static implicit operator File(string fileContent)
                => new File(fileContent);

            /// <summary>
            /// Creates a builder that will write the given content to a file,
            /// using the given encoding.
            /// </summary>
            /// <param name="fileContent">The content to write.</param>
            /// <param name="encoding">The encoding to use.</param>
            public File(string fileContent, Encoding encoding)
            {
                _fileContent = fileContent;
                _encoding = encoding;
            }

            /// <summary>
            /// Creates a <see cref="Builder"/> that will write the given content to a file,
            /// using the default (no-BOM UTF-8) encoding.
            /// </summary>
            /// <param name="fileContent">The content to write.</param>
            public File(string fileContent)
                : this(fileContent, DefaultEncoding)
            {
            }

            // no-BOM UTF-8 is the default
            private static readonly UTF8Encoding DefaultEncoding = new UTF8Encoding(false, true);

            /// <summary>
            /// Creates a file with the desired content at the path.
            /// </summary>
            /// <remarks>
            /// If the file already exists it will be overwritten.
            /// </remarks>
            /// <param name="path">Where to create the file.</param>
            public void Build(AnyFilePath path)
                => Build(path.ToString());

            /// <summary>
            /// Creates a file with the desired content at the path.
            /// </summary>
            /// <remarks>
            /// If the file already exists it will be overwritten.
            /// </remarks>
            /// <param name="path">Where to create the file.</param>
            public override void Build(string path)
                => System.IO.File.WriteAllText(path, _fileContent, _encoding);

            /// <summary>
            /// Builds the file in  a temporary file. When the <see cref="TemporaryFile"/> returned
            /// from this method is disposed, the file will be deleted.
            /// </summary>
            /// <returns>A <see cref="TemporaryFile"/> that controls the lifetime of the file.</returns>
            public TemporaryFile BuildTemporary()
            {
                var tmpFile = new TemporaryFile();
                try
                {
                    Build(tmpFile.File);
                    return tmpFile;
                }
                catch
                {
                    // on failure, cleanup
                    try
                    {
                        tmpFile.Dispose();
                    }
                    catch
                    {
                        // make sure we don't throw a secondary exception
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates a directory at the given path.
        /// </summary>
        public sealed class Directory : Builder, IDictionary<string, Builder>
        {
            private readonly Dictionary<string, Builder> _contents = new Dictionary<string, Builder>();
            private ICollection<KeyValuePair<string, Builder>> ContentsCollection => _contents;

            /// <summary>
            /// Builds the desired contents in the current directory.
            /// </summary>
            public void Build() => Build(System.IO.Directory.GetCurrentDirectory());

            /// <summary>
            /// Builds the directory with the desired contents at the given path.
            /// </summary>
            /// <remarks>If the directory already exists, files will be created inside it.</remarks>
            /// <param name="path">Where to build the directory.</param>
            public void Build(AnyDirectoryPath path)
                => Build(path.ToString());

            /// <summary>
            /// Builds the directory with the desired contents at the given path.
            /// </summary>
            /// <remarks>If the directory already exists, files will be created inside it.</remarks>
            /// <param name="path">Where to build the directory.</param>
            public override void Build(string path)
            {
                System.IO.Directory.CreateDirectory(path);

                foreach (var content in _contents)
                {
                    var p = Path.Combine(path, content.Key);
                    content.Value.Build(p);
                }
            }

            /// <summary>
            /// Builds the directory in a temporary directory. When the returned <see cref="TemporaryDirectory"/>
            /// is disposed, the directory (and its contents) will be deleted.
            /// </summary>
            /// <returns>An <see cref="TemporaryDirectory"/> that controls the lifetime of the directory.</returns>
            public TemporaryDirectory BuildTemporary()
            {
                var tmpDir = new TemporaryDirectory();
                try
                {
                    Build(tmpDir.Directory);
                    return tmpDir;
                }
                catch
                {
                    // on failure, cleanup
                    try
                    {
                        tmpDir.Dispose();
                    }
                    catch
                    {
                        // make sure we don't throw a secondary exception
                    }
                    throw;
                }
            }
            
            /// <summary>
            /// Returns <c>true</c> if there is current a <see cref="Builder"/> for the given path.
            /// </summary>
            public bool ContainsKey(string path) => _contents.ContainsKey(path);

            /// <summary>
            /// Removes the <see cref="Builder"/> for the given path.
            /// </summary>
            /// <returns><c>true</c> if one was removed</returns>
            public bool Remove(string path) => _contents.Remove(path);

            /// <summary>
            /// Adds a <see cref="Builder"/> for the given path.
            /// </summary>
            public void Add(string path, Builder builder) => _contents.Add(path, builder);

            /// <summary>
            /// Attempts to retrieve the <see cref="Builder"/> for the given path.
            /// </summary>
            public bool TryGetValue(string path, out Builder builder)
                => _contents.TryGetValue(path, out builder);

            /// <summary>
            /// Gets or sets the <see cref="Builder"/> for the given path.
            /// </summary>
            public Builder this[string path]
            {
                set => _contents[path] = value;
                get => _contents[path];
            }

            /// <summary>
            /// Sets the <see cref="File"/> for the given path.
            /// </summary>
            public File this[RelativeFilePath path]
            {
                set => _contents[path.ToString()] = value;
            }

            /// <summary>
            /// Sets the <see cref="Directory"/> for the given path.
            /// </summary>
            public Directory this[RelativeDirectoryPath path]
            {
                set => _contents[path.ToString()] = value;
            }

            /// <summary>
            /// Retrieves a list of all the paths that <see cref="Builder"/>s are registered for,
            /// in the current <see cref="Directory"/>.
            /// </summary>
            public ICollection<string> Keys => _contents.Keys;

            /// <summary>
            /// Retrieves all the <see cref="Builder"/>s that are registered for
            /// the current <see cref="Directory"/>.
            /// </summary>
            public ICollection<Builder> Values => _contents.Values;

            /// <summary>
            /// Removes all <see cref="Builder"/>s registered for the current <see cref="Directory"/>.
            /// </summary>
            public void Clear() => _contents.Clear();

            /// <summary>
            /// Gets the number of <see cref="Builder"/>s registered for the current <see cref="Directory"/>.
            /// </summary>
            public int Count => _contents.Count;

            /// <summary>
            /// Always <c>false</c>.
            /// </summary>
            public bool IsReadOnly => false;
            
            IEnumerator IEnumerable.GetEnumerator() => _contents.GetEnumerator();

            IEnumerator<KeyValuePair<string, Builder>> IEnumerable<KeyValuePair<string, Builder>>.GetEnumerator()
                => _contents.GetEnumerator();

            void ICollection<KeyValuePair<string, Builder>>.Add(KeyValuePair<string, Builder> item)
                => ContentsCollection.Add(item);

            bool ICollection<KeyValuePair<string, Builder>>.Contains(KeyValuePair<string, Builder> item)
                => ContentsCollection.Contains(item);

            void ICollection<KeyValuePair<string, Builder>>.CopyTo(KeyValuePair<string, Builder>[] array, int arrayIndex)
                => ContentsCollection.CopyTo(array, arrayIndex);

            bool ICollection<KeyValuePair<string, Builder>>.Remove(KeyValuePair<string, Builder> item)
                => ContentsCollection.Remove(item);
        }
    }
}