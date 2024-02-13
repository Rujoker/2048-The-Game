namespace _2048ConsoleEdition.Gameplay;

public interface IScoreProvider
{
    int Score { get; }
    int BestScore { get; }
}