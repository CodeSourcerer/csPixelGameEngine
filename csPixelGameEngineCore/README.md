# csPixelGameEngine
This is a C# port of the [olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine). It is currently roughly up to date
with v2.28 of the olcPixelGameEngine.

# Introduction / Background
I started this port for myself while following a set of video tutorials by [javidx9](https://www.youtube.com/c/javidx9) for creating a NES 
emulator. I wanted to recreate the same work in C# to see if it would be a viable language for such a thing, and well, also because I mostly forgot 
how to use C++. This meant taking the existing Pixel Game Engine he developed in C++ and porting it to C#. I see other people asking if there is
a C# port fairly often, so I decided to share what I did. 

This is not an "official" port of the original code, nor do I claim it will completely line up with the original code. In fact, it likely will
never line up with the original code because C++ is just fundamentally different from C# in some ways that I cannot make the interfaces perfectly
match. Also, the Pixel Game Engine gets updated quite regularly and it is challenging for me to keep up with all of those changes myself.

# Building
This engine is currently set up to target .NET 8. You can include the `csPixelGameEngineCore` project in your solution for you to 
reference, or build it separately and include the compiled .dll file in your project. I do not currently have this project in nuget, but if
there is a demand for it, I will consider it.

# Other Projects
## PixelGameEngineCoreTest
This is a demo project meant to test / demonstrate the capabilities of the engine. I use it as my personal sandbox, quite honestly, so it may be a little messy :smile:

## csPixelGameEngineCoreTests
This is a unit test project that is sadly too neglected.

*Note*: Other projects are temporary and will probably be removed at a later date.