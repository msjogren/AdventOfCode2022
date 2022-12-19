const int CaveSize = 1000;
const int SandOriginX = 500, SandOriginY = 0;

var input = File.ReadAllLines("input.txt");

(char[,], int) Parse(IEnumerable<string> input)
{
    var cave = new char[CaveSize, CaveSize];
    int maxY = -1;

    foreach (string line in input)
    {
        var segments = line.Split(" -> ").Select(coords => {
            int comma = coords.IndexOf(','),
            x = int.Parse(coords[0..comma]),
            y = int.Parse(coords[(comma+1)..]);
            return (x, y);
        });

        maxY = Math.Max(maxY, segments.Max(s => s.y));

        foreach (var coordPair in segments.Zip(segments.Skip(1)))
        {
            int deltaX = Math.Sign(coordPair.Second.x - coordPair.First.x),
                deltaY = Math.Sign(coordPair.Second.y - coordPair.First.y);
            int x = coordPair.First.x, y = coordPair.First.y;
            do
            {
                cave[x, y] = '#';
                x += deltaX;
                y += deltaY;
            } while (x != coordPair.Second.x || y != coordPair.Second.y);
            cave[x, y] = '#';
        }
    }

    return (cave, maxY);
}

int Solve(char[,] cave, int height)
{
    for (int sandUnits = 1; ; sandUnits++)
    {
        int sandX = SandOriginX, sandY = SandOriginY;
        while (sandY <= height)
        {
            if (cave[sandX, sandY + 1] == 0)
            {
                sandY++;
            }
            else if (cave[sandX - 1, sandY + 1] == 0)
            {
                sandY++;
                sandX--;
            }
            else if (cave[sandX + 1, sandY + 1] == 0)
            {
                sandY++;
                sandX++;
            }
            else
            {
                cave[sandX, sandY] = 'o';
                break;
            }
        }

        if (sandY >= height)
        {
            return sandUnits - 1;   // Don't count last overflowing unit
        }
        else if (sandX == SandOriginX && sandY == SandOriginY)
        {
            return sandUnits;
        }
    }
}

// Part 1
var (cave, maxY) = Parse(input);
Console.WriteLine(Solve(cave, maxY));

// Part 2
var (cave2, maxY2) = Parse(input.Append($"0,{maxY + 2} -> {CaveSize-1},{maxY + 2}"));
Console.WriteLine(Solve(cave2, maxY2));