using System;
using System.Collections.Generic;

namespace Pathy
{
    /// <summary>
    /// Contains instances of <see cref="IComparer{T}"/> for paths.
    /// </summary>
    public static class PathComparer
    {
        /// <summary>
        /// Gets a comparer that compares paths "logically".
        /// </summary>
        /// <typeparam name="T">The type of paths to compare.</typeparam>
        /// <returns>A new instance of <see cref="IComparer{T}"/>.</returns>
        public static IComparer<T> Logical<T>() where T : AnyPath =>
            new LogicalPathComparer<T>();

        /// <summary>
        /// Gets a comparer that compares paths in the default manner
        /// (i.e. ASCII-betically, and ignoring case).
        /// </summary>
        /// <typeparam name="T">The type of paths to compare.</typeparam>
        /// <returns>A new instance of <see cref="IComparer{T}"/>.</returns>
        public static IComparer<T> Default<T>() where T : AnyPath =>
            new DefaultPathComparer<T>();
    }

    internal sealed class LogicalPathComparer<T> : IComparer<T> where T : AnyPath
    {
        public int Compare(T x, T y) =>
            SafeNativeMethods.StrCmpLogicalW(x.ToString(), y.ToString());
    }

    internal sealed class DefaultPathComparer<T> : IComparer<T> where T : AnyPath
    {
        public int Compare(T x, T y) =>
            StringComparer.OrdinalIgnoreCase.Compare(x.ToString(), y.ToString());
    }
}
