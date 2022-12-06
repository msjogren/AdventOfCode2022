(IList<Stack<char>> stacks, IList<(int from, int to, int count)> moves) ParseInput()
{
    const int ColumnWidth = 4;
    var input = File.ReadAllLines("input.txt");
    int emptyRow = Array.IndexOf(input, "");
    int numberOfStacks = (input[0].Length + 1) / ColumnWidth;

    var stacks = new Stack<char>[numberOfStacks];
    for (int i = 0; i < numberOfStacks; i++)
    {
        stacks[i] = new Stack<char>();
    }    
    for (int row = emptyRow - 2; row >= 0; row--)
    {
        for (int col = 1, i = 0; col < input[row].Length; col += ColumnWidth, i++)
        {
            char crate = input[row][col];
            if (crate != ' ') stacks[i].Push(crate);
        }
    }

    var moves = new List<(int, int, int)>();
    foreach (string instruction in input.Skip(emptyRow + 1))
    {
        var parts = instruction.Split(' ');
        int count = int.Parse(parts[1]),
            from = int.Parse(parts[3]) - 1,
            to = int.Parse(parts[5]) - 1;

        moves.Add((from, to, count));
    }

    return (stacks, moves);
}

void Solve(Action<Stack<char>, Stack<char>, int> mover)
{
    var input = ParseInput();

    foreach (var move in input.moves)
    {
        mover(input.stacks[move.from], input.stacks[move.to], move.count);
    }

    Console.WriteLine(new string(input.stacks.Select(s => s.Peek()).ToArray()));
}

// Part 1
void CrateMover9000<T>(Stack<T> from, Stack<T> to, int count) 
{
    while(count-- > 0)
    {
        to.Push(from.Pop());
    }
}

Solve(CrateMover9000);

// Part 2
void CrateMover9001<T>(Stack<T> from, Stack<T> to, int count) 
{
    var tmp = new Stack<T>();
    while (count-- > 0)
    {
        tmp.Push(from.Pop());
    }
    while (tmp.TryPop(out T crate))
    {
        to.Push(crate);
    }
}

Solve(CrateMover9001);
