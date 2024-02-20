using _2048ConsoleEdition.Gameplay;

namespace _2048ConsoleEdition.Input;

public class ConsoleInput : IInput
{
    public Action<Direction>? OnMoveRequested { get; set; }
    public Action? OnQuitRequested { get; set; }
    public Action? OnRestartRequested { get; set; }
    public Action? OnConfirmed { get; set; }
    public Action? OnCanceled { get; set; }
    public Action? OnForcedQuit { get; set; }

    public ConsoleInput()
    {
        Console.CancelKeyPress += (sender, args) => OnForcedQuit?.Invoke();
    }
    
    public void WaitForUserInput()
    {
        var pressedButton = Console.ReadKey(true);
        
        switch (pressedButton.Key)
        {
            case ConsoleKey.UpArrow:
                OnMoveRequested?.Invoke(Direction.Up);
                break;
        
            case ConsoleKey.DownArrow:
                OnMoveRequested?.Invoke(Direction.Down);
                break;
        
            case ConsoleKey.LeftArrow:
                OnMoveRequested?.Invoke(Direction.Left);
                break;
        
            case ConsoleKey.RightArrow:
                OnMoveRequested?.Invoke(Direction.Right);
                break;
        
            case ConsoleKey.R:
                OnRestartRequested?.Invoke();
                break;
            
            case ConsoleKey.Q:
                OnQuitRequested?.Invoke();
                break;
            
            case ConsoleKey.Y: 
                OnConfirmed?.Invoke();
                break;
            
            case ConsoleKey.N:
                OnCanceled?.Invoke();
                break;
        }
    }
}