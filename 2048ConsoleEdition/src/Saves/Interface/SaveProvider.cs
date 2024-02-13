using System.Text.Json;

namespace _2048ConsoleEdition.Saves;

public class SaveProvider : ISaveProvider
{
    private const string Path = "save.json";
    
    private SaveData? _save = new SaveData();
    
    public async Task InitializeAsync()
    {
        await ReadAsync();
    }

    public async Task SaveAsync()
    {
        await WriteAsync();
    }

    public int BestScore
    {
        get => _save?.Score ?? 0;
        set
        {
            if (_save != null)
            {
                _save.Score = value;
            }
        }
    }

    private async Task ReadAsync()
    {
        try
        {
            await using var stream = new FileStream(Path, FileMode.OpenOrCreate); 
            _save = await JsonSerializer.DeserializeAsync<SaveData>(stream);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error of reading file");
        }
    }

    private async Task WriteAsync()
    {
        try
        { 
            await using var stream = new FileStream(Path, FileMode.Truncate);
            await JsonSerializer.SerializeAsync(stream, _save);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error of writing file");
        }
    }
}