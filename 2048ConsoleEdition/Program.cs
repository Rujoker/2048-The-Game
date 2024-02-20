using _2048ConsoleEdition;
using _2048ConsoleEdition.Graphics;
using _2048ConsoleEdition.Input;
using _2048ConsoleEdition.Saves;

var drawer = new ConsoleDrawer();
var input = new ConsoleInput();
var saveProvider = new SaveProvider();

Game game;

do
{
    game = new Game(drawer, input, saveProvider);
    await game.RunAsync();
} 
while (game.IsPlaying);