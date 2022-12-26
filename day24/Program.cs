var input = File.ReadAllLines("input.txt");
int width = input[0].Length - 2, height = input.Length - 2;
var entrancePos = (x: input[0].IndexOf('.') - 1, y: -1);
var exitPos = (x: input.Last().IndexOf('.') - 1, y: height);
var winds = new char[width, height];

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        winds[x, y] = input[y + 1][x + 1];
    }
}

int BfsShortestPathToExit(bool part2)
{
    var seen = new HashSet<(int time, int x, int y, int trip)>();
    var queue = new Queue<(int time, int x, int y, int trip)>();
    queue.Enqueue((0, entrancePos.x, entrancePos.y, 1));

    while (queue.TryDequeue(out var state))
    {
        if (seen.Contains(state)) continue;
        seen.Add(state);

        int trip = state.trip;
        int time = state.time + 1;

        if (state.x == exitPos.x && state.y == exitPos.y)
        {
            if ((part2 && trip == 3) || !part2)
            {
                return state.time;
            }
            else
            {
                trip = 2;
            }
        }
        else if (state.x == entrancePos.x && state.y == entrancePos.y && trip == 2)
        {
            trip = 3;
        }

        var possibleMoves = new (int dx, int dy)[] {(1, 0), (0, 1), (0, 0), (-1, 0), (0, -1)}
            .Select(offset => (x: state.x + offset.dx, y: state.y + offset.dy))
            .Where(pt => (pt.x >= 0 && pt.y >= 0 && pt.x < width && pt.y < height) ||
                         (pt.x == exitPos.x && pt.y == exitPos.y) ||
                         (pt.x == entrancePos.x && pt.y == entrancePos.y));

        foreach (var move in possibleMoves)
        {
            bool blockedByWind = false;
            
            if (move != exitPos && move != entrancePos)
            {
                blockedByWind = winds[((move.x - time) % width + width) % width, move.y] == '>' ||
                                winds[((move.x + time) % width + width) % width, move.y] == '<' || 
                                winds[move.x, ((move.y - time) % height + height) % height] == 'v' || 
                                winds[move.x, ((move.y + time) % height + height) % height] == '^';
            }
            if (!blockedByWind) queue.Enqueue((time, move.x, move.y, trip));
        }
    }

    return -1;
}

Console.WriteLine(BfsShortestPathToExit(false));
Console.WriteLine(BfsShortestPathToExit(true));