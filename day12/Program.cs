var input = File.ReadAllLines("input.txt");
int width = input[0].Length, height = input.Length;
var startPos = (x: -1, y: -1);
var endPos = (x: -1, y: -1);

for (int y = 0; y < input.Length && (startPos.y == -1 || endPos.y == -1); y++)
{
    int sx = input[y].IndexOf('S'),
        ex = input[y].IndexOf('E');
    if (sx != -1)
    {
        startPos = (sx, y);
        input[y] = input[y].Replace('S', 'a');
    }

    if (ex != -1)
    {
        endPos = (ex, y);
        input[y] = input[y].Replace('E', 'z');
    }
}

int BFS(string[] grid, (int x, int y) from, Func<(int x, int y), bool> isEnd, Func<(int x, int y), (int x, int y), bool> canNavigate)
{
    var seen = new HashSet<(int x, int y)>();
    var queue = new Queue<((int x, int y) pos, int steps)>();
    queue.Enqueue((from, 0));

    while (queue.TryDequeue(out var current))
    {
        if (!seen.Add(current.pos)) continue;

        if (isEnd(current.pos)) return current.steps;

        foreach (var offset in new[] {(x: -1, y: 0), (x: 1, y: 0), (x: 0, y: -1), (x: 0, y: 1)})
        {
            var destination = (x: current.pos.x + offset.x, y: (current.pos.y + offset.y));

            if (destination.x < 0 || destination.y < 0 || destination.x >= width || destination.y >= height) continue;
            if (canNavigate(current.pos, destination))
            {
                queue.Enqueue((destination, current.steps + 1));
            }
        }
    }

    return -1;
}

// Part 1
Console.WriteLine(BFS(input, startPos,
                      pos => pos == endPos,
                      (from, to) => input[to.y][to.x] <= input[from.y][from.x] + 1));

// Part 2
Console.WriteLine(BFS(input, endPos, 
                      pos => input[pos.y][pos.x] == 'a',
                      (from, to) => input[from.y][from.x] <= input[to.y][to.x] + 1));
