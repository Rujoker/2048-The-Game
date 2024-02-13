namespace _2048ConsoleEdition.Saves;

public interface ISaveProvider
{
    int BestScore { get; set; }

    Task InitializeAsync();
    Task SaveAsync();
}