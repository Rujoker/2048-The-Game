using _2048ConsoleEdition.Gameplay;

namespace _2048ConsoleEdition.Input;

public interface IInput
{
    Action<Direction>? OnMoveRequested { get; set; }
    Action? OnQuitRequested { get; set; }
    Action? OnRestartRequested { get; set; }
    Action? OnConfirmed { get; set; }
    Action? OnCanceled { get; set; }

    void WaitForUserInput();
}