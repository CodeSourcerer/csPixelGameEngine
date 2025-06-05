# csPixelGameEngine
This is a C# port of the [olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine). It is roughly up to date
with v2.28 of the olcPixelGameEngine.

# Introduction
I started this port for myself while following a set of video tutorials by [javidx9](https://www.youtube.com/c/javidx9) for creating a NES 
emulator. I wanted to recreate the same work in C# to see if it would be a viable language for such a thing, and because I mostly forgot 
how to use C++. This meant taking the existing PixelGameEngine he developed and porting it to C# as well. I thought others may find it helpful 
as well, so here is my work!

This is not an official port of the original clone, nor do I claim it will completely line up with the original code. In fact, it is very
difficult to achieve because C++ simply lets you do things that C# does not. I have done my best to document where I do things differently 
when it is relevant.

# Building
This engine is currently set up to target .NET 8. You can include the csPixelGameEngineCore project in your solution for you to 
reference, or build it separately and include the compiled .dll file in your project. I do not currently have this project in nuget, but if
there is a demand for it, I will consider it.

# Other Projects
### PixelGameEngineCoreTest
This is a demo project meant to test / demonstrate the capabilities of the engine.

### csPixelGameEngineCoreTests
This is a unit test project that is sadly too neglected.

*Note*: Other projects are temporary and will probably be removed at a later date.