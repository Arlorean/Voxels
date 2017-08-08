# MagicaVoxel Windows Explorer Thumbnails

The [Voxels.Setup.exe](https://github.com/Arlorean/Voxels/releases/download/v1.1/Voxels.Setup.exe) provides Windows Explorer Thumbnails for MagicaVoxel .vox files:

![Windows Explorer Thumbnails](Voxels.Website/WindowsExplorer.png)

![mmmm Thumbnails](Voxels.Website/mmmm.png)

The library uses [SkiaSharp](https://github.com/mono/SkiaSharp#using-skiasharp) which requires [Visual C++ Redistributable for Visual Studio 2015](https://www.microsoft.com/en-us/download/details.aspx?id=48145) to be installed. The exe setup does this for you.

# PNG and SVG output

The Voxels.CommandLine.exe tool converts .vox files to .png and .svg (512x512). Here is my example [wizard.vox](Voxels.CommandLine/wizard.vox) file converted:

PNG             |  SVG
----------------|-------------------------
![PNG](Voxels.Website/wizard.png)  |  ![SVG](https://cdn.rawgit.com/Arlorean/Voxels/df6f605a/Voxels.Website/wizard.svg)

# Voxels

C# utilities for reading/writing/rendering [MagicaVoxel](https://ephtracy.github.io/) [.vox files](https://github.com/ephtracy/voxel-model/blob/master/MagicaVoxel-file-format-vox.txt). 

# Command Line Build

1. Install ``Visual Studio 2017``
1. Open ``Voxels.sln``
1. Set ``Voxels.CommandLine`` as the startup project
1. Set ``Debug -> Command line arguments`` to be ``wizard.vox``
1. Press ``Start`` in Visual Studio 2017
1. Open Windows Explorer on the ``Voxels.CommandLine\bin\Debug directory``
1. There should be two new files: ``wizard.png`` and ``wizard.svg``

# Setup Build

1. Install ``Visual Studio 2017``
1. Open ``Voxels.sln``
1. Install [WiX Toolset](http://wixtoolset.org/) v3.11
1. Install [Wix Toolset Visual Studio 2017 Extension](https://marketplace.visualstudio.com/items?itemName=RobMensching.WixToolsetVisualStudio2017Extension)
1. Build ``Voxels.Setup`` to create the [Voxels.Setup.exe](https://github.com/Arlorean/Voxels/releases/download/v1.1/Voxels.Setup.exe) setup file. 

# Third Party Credits

1. [SkiaSharp](https://github.com/mono/SkiaSharp) - Xamarin C# wrapper for Google's Skia 2D rendering library
1. [SharpShell](https://github.com/dwmkerr/sharpshell) - Dave Kerr's ShellExtensions Library for .NET
1. [SharpShellTools](https://github.com/dwmkerr/sharpshell) - Dave Kerr's ShellExtensions Tools for .NET
1. [WiX Toolset](http://wixtoolset.org/) - ~~Simple~~ XML based windows installer scripting
1. [Ambient Occlusion](https://0fps.net/2013/07/03/ambient-occlusion-for-minecraft-like-worlds/) algorithm by [Mikola Lysenko](https://github.com/mikolalysenko).
1. [Wix VC++ 2015 Setup](https://gist.github.com/nathancorvussolis/6852ba282647aeb0c5c00e742e28eb48) gist for installing VC++ 2015 dlls.

The 3x3x3.vox, 8x8x8.vox files are directly from the [MagicaVoxel](https://ephtracy.github.io/) distribution for authentic testing.

# TODO

* Add shell context menus to export PNG/SVG interactively 
