using System;
using System.Collections.Generic;

namespace Pathy
{
    public static class PathComparer
    {
        public static IComparer<T> Logical<T>() where T : AnyPath =>
            new LogicalPathComparer<T>();

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
