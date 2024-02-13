This is another one simple implementation of the game 2048.
It's a console application that was built with .Net 8.0.

The game is divided into modules. So it has these parts: 
- gameplay core,
- user keyboard input,
- console output,
- local save system,
- entry point in Program.cs.

This approach allows changing specific modules. 
For example you can take the core module and use it with a custom UI framework or add cloud saving.
