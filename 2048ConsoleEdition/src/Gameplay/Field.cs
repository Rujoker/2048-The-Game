namespace _2048ConsoleEdition.Gameplay;

public class Field
{
    public readonly int[,] Cells;
    public FieldState State = FieldState.Normal;
    
    private readonly Random _random = new Random();
    
    private int RowCount => Cells.GetLength(0);
    private int ColCount => Cells.GetLength(1);

    public Field(int rowCount, int columnCount)
    {
        Cells = new int[rowCount, columnCount];
    }
    
    public void PutNewValue()
    {
        var emptyCells = new List<(int, int)>();
        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColCount; j++)
            {
                if (Cells[i, j] == 0)
                {
                    emptyCells.Add((i, j));
                }
            }
        }

        if (emptyCells.Count == 0)
        {
            return;
        }

        var cellToPutValue = _random.Next(0, emptyCells.Count);
        Cells[emptyCells[cellToPutValue].Item1, emptyCells[cellToPutValue].Item2] = 
            _random.NextSingle() < Configuration.StartValueBigChance ? 4 : 2;
    }
    
    public void Move(Direction direction, out int score)
    {
        score = 0;

        if (State != FieldState.Normal)
        {
            return;
        }
        
        var isHorizontal = direction is Direction.Left or Direction.Right;
        var isIncreasing = direction is Direction.Left or Direction.Up;

        var outterCount = isHorizontal ? RowCount : ColCount;
        var innerCount = isHorizontal ? ColCount : RowCount;
        var innerStart = isIncreasing ? 0 : innerCount - 1;

        for (var i = 0; i < outterCount; i++)
        {
            for (var j = innerStart; IsValidIndex(j, innerCount); j = ShiftIndexBack(j, isIncreasing))
            {
                if (GetValue(Cells, i, j, isHorizontal) == 0)
                {
                    continue;
                }

                var k = ShiftIndex(j, isIncreasing);
                while (IsValidIndex(k, innerCount) && GetValue(Cells, i, k, isHorizontal) == 0)
                {
                    k = ShiftIndex(k, isIncreasing);
                }

                if (IsValidIndex(k, innerCount) && GetValue(Cells, i, k, isHorizontal) == GetValue(Cells, i, j, isHorizontal))
                {
                    var newValue = GetValue(Cells, i, k, isHorizontal) * 2;
                    SetValue(Cells, i, k, newValue, isHorizontal);
                    SetValue(Cells, i, j, 0, isHorizontal);

                    State = FieldState.Moved;
                    score += newValue;
                }
                else
                {
                    k = ShiftIndexBack(k, isIncreasing);
                    if (k != j)
                    {
                        State = FieldState.Moved; 
                    }

                    var value = GetValue(Cells, i, j, isHorizontal);
                    SetValue(Cells, i, j, 0, isHorizontal);
                    SetValue(Cells, i, k, value, isHorizontal);
                }
            }
        }

        return;
        
        static int ShiftIndex(int innerIndex, bool isIncreasing) => isIncreasing ? innerIndex - 1 : innerIndex + 1;
        static int ShiftIndexBack(int innerIndex, bool isIncreasing) => isIncreasing ? innerIndex + 1 : innerIndex - 1;
        static int GetValue(int[,] cells, int i, int j, bool isHorizontal) => isHorizontal ? cells[i, j] : cells[j, i];
        static void SetValue(int[,] cells, int i, int j, int v, bool isHorizontal)
        {
            if (isHorizontal)
            {
                cells[i, j] = v;
            }
            else
            {
                cells[j, i] = v;
            }
        }
        static bool IsValidIndex(int index, int innerCount) => 0 <= index && index <= innerCount - 1;
    }

    public void Update()
    {
        if (State is not FieldState.Moved)
        {
            return;
        }

        if (IsWin())
        {
            State = FieldState.Victory;
            return;
        }

        PutNewValue();

        if (!HasMoves())
        {
            State = FieldState.Defeat;
            return;
        }

        State = FieldState.Normal;
    }

    private bool IsWin()
    {
        foreach (var cell in Cells)
        {
            if (cell >= Configuration.MaxValue)
            {
                return true;
            }
        }
        return false;
    }

    private bool HasMoves()
    {
        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColCount; j++)
            {
                if (Cells[i, j] == 0)
                {
                    return true;
                }
                                
                if (i < RowCount - 1 && Cells[i, j] == Cells[i + 1, j])
                {
                    return true;
                }

                if (j < ColCount - 1 && Cells[i, j] == Cells[i, j + 1])
                {
                    return true;
                }
            }
        }

        return false;
    }
}