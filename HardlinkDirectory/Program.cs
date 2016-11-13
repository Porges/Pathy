using System;
using System.Collections.Generic;
using System.Linq;
using Pathy;

namespace Hardlink
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length != 2)
                {
                    Console.Error.WriteLine("Two arguments required: <source directory> <destination directory>.");
                    return 1;
                }

                var source = AnyDirectoryPath.From(args[0]);
                var dest = AnyDirectoryPath.From(args[1]);

                HardlinkDir(source, dest);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        private static void HardlinkDir(AnyDirectoryPath fromRoot, AnyDirectoryPath toRoot)
        {
            var subDirsToVisit = new Stack<RelativeDirectoryPath>();
            subDirsToVisit.Push(RelativeDirectoryPath.From("."));

            while (subDirsToVisit.Any())
            {
                var subDir = subDirsToVisit.Pop();

                var fromDir = fromRoot / subDir;
                var toDir = toRoot / subDir;
                toDir.Create();

                foreach (var file in fromDir.EnumerateFiles())
                {
                    file.CreateHardLinkAs(toDir / file.FileName);
                }

                foreach (var dir in fromDir.EnumerateDirectories())
                {
                    subDirsToVisit.Push(subDir / dir.DirectoryName);
                }
            }
        }
    }
}
