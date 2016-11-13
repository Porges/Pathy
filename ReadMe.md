# Pathy [![Build status](https://ci.appveyor.com/api/projects/status/awr5tpw9n1wj17ax?svg=true)](https://ci.appveyor.com/project/Porges/pathy)


Pathy is a library for safe, typed path manipulation.

## Type axes

Paths are either *absolute* or *relative* and point at *directories* or *files*.


Combining these two axes results in the following hierarchy:

* `AnyPath` – any path
  * `AnyFilePath` – any path to a file
    * `FilePath` – an absolute path to a file
    * `RelativeFilePath` – a relative path to a file
      * `FileName` – a relative path to a file, without directory components
  * `AnyDirectoryPath` – any path to a directory
    * `DirectoryPath` – an absolute path to a directory
    * `RelativeDirectoryPath` – a relative path to a directory

:warning: Pathy *has opinions*, so it gives shorter names to absolute paths to encourage their use.

## The `/` operator

The `/` operator is the main reason that Pathy exists. It allows you to combine paths in a safe manner.

It can be used with the following combinations of types:

|     `below / right`      |  RelativeDirectoryPath   |  RelativeFilePath   |
| :----------------------: | :----------------------: | :-----------------: |
| (Absolute) DirectoryPath | (Absolute) DirectoryPath | (Absolute) FilePath |
|  RelativeDirectoryPath   |  RelativeDirectoryPath   |  RelativeFilePath   |
|     AnyDirectoryPath     |     AnyDirectoryPath     |     AnyFilePath     |

## Long paths

Pathy is based on .NET 4.6.2, so supports long paths out of the box.

## Formatting

Pathy includes support for formatting paths to particular widths. If the requested width is less than the path length, it will be truncated using ellipses. For example:

```csharp
    var path = FilePath.From(@"C:\path\to\a\file.txt")
    Console.WriteLine($"{path:20}");
```

The output of this is is `C:\path\...\file.txt`.

> ​ :information_source: This feature is powered by Win32 functions, so matches what you see in `explorer.exe`.

## Comparisons

Only paths of the exact same type should be compared. Pathy does not support comparing a relative path to an absolute one, as in general this doesn't make sense. Unfortunately .NET places `GetHashCode` & `Equals` on the root `object` type, so there's no sane way to prevent you from doing this (just as there’s no way to prevent you from asking `10.Equals("10")`). Just don’t.

The one exception to this rule is that `FileName`s can be compared with `RelativeFilePath`s.

## Sorting

Pathy supports logical sorting. This means that numeric groups in paths are compared by their numeric value and not their ASCII-betical value.

Logical sorting is not the default since its behaviour may change, so it should only be used for display purposes. To use logical sorting, use `PathComparer.Logical<T>()`



> :information_source: This feature is also powered by Windows, so matches what you see in `explorer.exe`.


## TODO

Relative paths are not normalized, so comparing them can have inconsistent results.