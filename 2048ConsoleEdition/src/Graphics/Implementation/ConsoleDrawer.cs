using _2048ConsoleEdition.Gameplay;

namespace _2048ConsoleEdition.Graphics;

public class ConsoleDrawer : IDrawer
{
    private const char VerticalDivider = '|';
    private const char HorizontalLine = '-';
    private const int CellLength = 4;
    
    public ConsoleDrawer()
    {
        Console.CursorVisible = false;
    }

    public void DrawLoader()
    {
        Prepare();
        Console.WriteLine("Loading... Please stand by...");
    }
    
    public void Draw(IScoreProvider scoreProvider, GameState gameState, Field field)
    {
        Prepare();
        DrawField(field);
        DrawGameplayMessages(scoreProvider, field.State);
        DrawServiceMessages(gameState);
    }

    private void Prepare()
    {
        Console.Clear();
    }

    private void DrawField(Field field)
    {
        var rowCount = field.Cells.GetLength(0);
        var colCount = field.Cells.GetLength(1);

        var horizontalSubDivider = $"{string.Join("", Enumerable.Repeat(HorizontalLine, CellLength))}";
        var horizontalDivider = VerticalDivider 
                                + string.Join(VerticalDivider, Enumerable.Repeat(horizontalSubDivider, colCount)) 
                                + VerticalDivider;

        Console.WriteLine(horizontalDivider);
        
        for (var i = 0; i < rowCount; i++)
        {
            for (var j = 0; j < colCount; j++)
            {
                Console.Write(VerticalDivider);
                DrawCell(field.Cells[i, j]);
            }
            
            Console.WriteLine(VerticalDivider);
            Console.WriteLine(horizontalDivider);
        }
    }

    private void DrawCell(int cellValue)
    {
        var cellInfo = GetCellInfo(cellValue);
        Console.ForegroundColor = cellInfo.Item2;
        Console.Write(cellInfo.Item1);
        Console.ResetColor();
    }

    private void DrawGameplayMessages(IScoreProvider scoreProvider, FieldState fieldState)
    {
        Console.WriteLine("Score: {0}", scoreProvider.Score);
        Console.WriteLine("BestScore: {0}", scoreProvider.BestScore);

        switch (fieldState)
        {
            case FieldState.Normal:
            case FieldState.Moved:
                Console.WriteLine("Arrows - move tiles. R - restart. Q - quit.");
                break;
            case FieldState.Defeat:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("YOU LOSE! R - try again. Q - quit.");
                Console.ResetColor();
                break;
            case FieldState.Victory:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("YOU WIN! R - new game. Q - quit.");
                Console.ResetColor();
                break;
        }
    }

    public void DrawServiceMessages(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.RestartRequested:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Try again? (Y/N)");
                Console.ResetColor();
                break;
            case GameState.QuitRequested:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Quit? (Y/N)");
                Console.ResetColor();
                break;
        }
    }

    private static (string, ConsoleColor) GetCellInfo(int value)
    {
        var result = ($"{value, CellLength}", ConsoleColor.White);
        switch (value)
        {
            case 0:
                result = ($"{"", CellLength}", ConsoleColor.White);
                break;
            case 2:
                result = ($"{value, CellLength}", ConsoleColor.DarkGray);
                break;
            case 4:
                result = ($"{value, CellLength}", ConsoleColor.Gray);
                break;
            case 8:
                result = ($"{value, CellLength}", ConsoleColor.White);
                break;
            case 16:
                result = ($"{value, CellLength}", ConsoleColor.DarkMagenta);
                break;
            case 32:
                result = ($"{value, CellLength}", ConsoleColor.Magenta);
                break;
            case 64:
                result = ($"{value, CellLength}", ConsoleColor.DarkRed);
                break;
            case 128:
                result = ($"{value, CellLength}", ConsoleColor.Red);
                break;
            case 256:
                result = ($"{value, CellLength}", ConsoleColor.DarkGreen);
                break;
            case 512:
                result = ($"{value, CellLength}", ConsoleColor.Green);
                break;
            case 1024:
                result = ($"{value, CellLength}", ConsoleColor.DarkYellow);
                break;
            case 2048:
                result = ($"{value, CellLength}", ConsoleColor.Yellow);
                break;
        }
        return result;
    }
}