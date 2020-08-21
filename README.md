# Bob Greenhands

A small game made to look and feel like old gardening simulators of the early 21st century.

## About the game

This game is supposed to look and feel like gardening simulators of the early 2000s, specifically the [Alice Greenfingers](https://www.alicegreenfingers.com/) series (which is also inspiration for the name of the game), while also adding more features and more fun elements into the game.
This game is in no way an official successor the the Alice Greenfingers series, I'm also not affiliated with Arcade Lab.

## Is the game already finished?

Hell no, it's still in very early developement and while you can """play""" it, right now there is not much besides planting strawberries and watch them grow and moving the main character, Bob, around.

## Requirements and supported architectures/operating systems

To run this game, you need an x64 processor (it might run on x86, it might run on ARM, but I really can't be bothered to also support those architectures, feel free to mess around with it though), a graphics card preferably from this century and around a hundred megabytes of storage. If your computer manages to run Windows 10 (or even Windows 7) or any modern GNU/Linux distribution, the game should run.
You also need [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) or higher installed on your device.

.NET Core also supports BSD and macOS but I can't guarantee support for both operating systems as I don't have an Apple device (and don't plan on either buying one or breaking Apple's EULA) nor any kind of BSD installed.

I test this game mainly on my desktop PC running Manjaro Linux with GNOME on an AMD Ryzen 5 1600 with an NVIDIA GTX 1060, on my notebook running Manjaro Linux with GNOME on an Intel i5-6200U with integrated Intel HD Graphics and also rarely on an older machine running Windows 10 on an AMD A6-5200 APU with integrated Radeon R3 Graphics.

**TL;DR:**

Requirements:

- [.NET Core Runtime 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) or higher if you have a prebuilt version of the game
- [.NET Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) (includes the Runtime) or higher for building the game
- ``cmake``, ``make`` and ``gcc`` for building FNA3D on GNU/Linux (and presumably macOS/OS X?)
- ``faudio``
- Visual Studio 2010 for building FNA3D on Windows - **[read more](https://github.com/FNA-XNA/FNA3D/blob/master/visualc/README)**

Fully supported operating systems and architectures:

- any modern GNU/Linux system
- Windows 7 or higher
- any x64 capable processors
- any somewhat modern GPU capable of doing 2D graphics
- any sound device that works with your operating system

Partially supported operating systems and architectures (it might work, it might not, but making it work is definitely not a priority):

- any macOS/OS X version
- any Windows version older than Windows 7
- any form of BSD
- any processor not supporting the x64 instruction set (i. e. old 32-bit processors, ARM processors)
- anything else not mentioned in the previous list

## Building and running the game

**Right now you have to build FNA3D yourself on Windows until I've figured out how to put that into the BobGreenhands.csproj! Read more [here](https://github.com/FNA-XNA/FNA3D/blob/master/visualc/README).**
Clone the repository **recursively** with ``git clone --recurse-submodules git@github.com:ManicRobot/bobgreenhands.git``, navigate into the newly created subfolder ``bobgreenhands/BobGreenhands`` and type ``dotnet run`` in your favorite terminal/command prompt. Proper installation is a thing that I'm not worried about just yet, I want to get the game in a functioning (and enjoyable) state before messing with details like that.
If it doesn't start the first time because it's unable to load shared library 'FNA3D', just try running ``dotnet run`` again. If the issue persists, see, if FNA3D has been compiled.

## Reporting bugs

You found a bug in the game or your game crashed? Report it by opening an issue! Navigate to the "Issues" tab on top of the page and click the green "New issue" button. Be sure to include crash reports and savegames (if available) in the issue so I can fix it faster. While you're there, if you have suggestions or ideas for the game, you can open another issue for that too.

## Acknowledgements

Thanks to

- [prime31](https://github.com/prime31) for building [Nez](https://github.com/prime31/Nez), an awesome Monogame/FNA 2D Framework
- [jamesjoplin](https://github.com/jamesjoplin) for the [.NET Core compatible fork of Nez](https://github.com/jamesjoplin/Nez)
- MarsFreak for helping out with the design
