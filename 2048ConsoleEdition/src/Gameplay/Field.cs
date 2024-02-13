namespace _2048ConsoleEdition.Gameplay;

public class Field
{
    public readonly int[,] Cells;
    public FieldState State = FieldState.Normal;
    
    private readonly Random _random = new Random();
    
    private int RowCount => Cells.GetLength(0);
    private int ColCount => Cells.GetLength(0);

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
        Cells[emptyCells[cellToPutValue].Item1, emptyCells[cellToPutValue].Item2] = 2;
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
            for (var j = innerStart; IsValidIndex(j); j = ShiftIndexBack(j))
            {
                if (GetValue(i, j) == 0)
                {
                    continue;
                }

                var k = ShiftIndex(j);
                while (IsValidIndex(k) && GetValue(i, k) == 0)
                {
                    k = ShiftIndex(k);
                }

                if (IsValidIndex(k) && GetValue(i, k) == GetValue(i, j))
                {
                    var newValue = GetValue(i, k) * 2;
                    SetValue(i, k, newValue);
                    SetValue(i, j, 0);

                    State = FieldState.Moved;
                    score += newValue;
                }
                else
                {
                    k = ShiftIndexBack(k);
                    if (k != j)
                    {
                        State = FieldState.Moved; 
                    }

                    var value = GetValue(i, j);
                    SetValue(i, j, 0);
                    SetValue(i, k, value);
                }
            }
        }

        return;
        
        int ShiftIndex(int innerIndex) => isIncreasing ? innerIndex - 1 : innerIndex + 1;
        int ShiftIndexBack(int innerIndex) => isIncreasing ? innerIndex + 1 : innerIndex - 1;
        int GetValue(int i, int j) => isHorizontal ? Cells[i, j] : Cells[j, i];
        void SetValue(int i, int j, int v)
        {
            if (isHorizontal)
            {
                Cells[i, j] = v;
            }
            else
            {
                Cells[j, i] = v;
            }
        }
        bool IsValidIndex(int index) => 0 <= index && index <= innerCount - 1;
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
        return Cells.Cast<int>().Any(cell => cell >= Configuration.MaxValue);
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