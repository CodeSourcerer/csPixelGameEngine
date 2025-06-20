# csPixelGameEngine
This is a C# port of the [olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine). It is currently roughly up to date
with v2.29 of the olcPixelGameEngine.

# Introduction / Background
I started this port for myself while following a set of video tutorials by [javidx9](https://www.youtube.com/c/javidx9) for creating a NES 
emulator. I wanted to recreate the same work in C# to see if it would be a viable language for such a thing, and well, also because I mostly forgot 
how to use C++. This meant taking the existing Pixel Game Engine he developed in C++ and porting it to C#. I see other people asking if there is
a C# port fairly often, so I decided to share what I did. 

This is not an "official" port of the original code, nor do I claim it will completely line up with the original code. In fact, it likely will
never line up with the original code because C++ is just fundamentally different from C# in some ways that I cannot make the interfaces perfectly
match. Also, the Pixel Game Engine gets updated quite regularly and it is challenging for me to keep up with all of those changes myself.

# Building

## Windows

This engine is currently set up to target .NET 8. You can include the `csPixelGameEngineCore` project in your solution for you to 
reference, or build it separately and include the compiled .dll file in your project. I do not currently have this project in nuget, but if
there is a demand for it, I will consider it.

## Ubuntu

### Prerequesites

First, you need to install the Microsoft Package Repository by running the following commands.
```bash
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
```

Then you need to install the dotnet sdk with the following commands.
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

Verify it's installed by running
```bash
dotnet --version
```
### Actually building

```bash
git clone https://github.com/CodeSourcerer/csPixelGameEngine

cd csPixelGameEngine

dotnet restore

dotnet build
```

Once that's finished, run the project by doing the following:

```bash
cd PixelGameEngineCoreTest

dotnet run
```

# Other Projects
## PixelGameEngineCoreTest
This is a demo project meant to test / demonstrate the capabilities of the engine. I use it as my personal sandbox, quite honestly, so it may be a little messy :smile:

## csPixelGameEngineCoreTests
This is a unit test project that is sadly too neglected.

*Note*: Other projects are temporary and will probably be removed at a later date.

# Features Not Yet Implemented

* Console functions
* "Dropped Files" stuff. 
* ImageLoader interface - I somehow missed this when updating v1 to v2. Will be done soon!
* Key caching stuff added in v2.29
* Other misc keyboard related functions added to v2.29
  * Honestly most of this stuff added can be accomplished today, but in slightly different ways.