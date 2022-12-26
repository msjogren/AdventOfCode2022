const int Right = 0,
          Down = 1,
          Left = 2,
          Up = 3;

var input = File.ReadAllLines("input.txt");
var gridLines = input[0..^2];
int height = gridLines.Length,
    width = gridLines.Max(line => line.Length);
var instructions = input.Last().Replace("R", " R ").Replace("L", " L ").Split(' ', StringSplitOptions.RemoveEmptyEntries);

bool IsWall(int x, int y) => gridLines[y][x] == '#';

int CalculatePassword((int x, int y) startPos, Func<int, int, int, (int facing, int x, int y)> transform)
{
    var currentPos = startPos;
    int facing = 0;
    foreach (var step in instructions!)
    {
        if (step == "L")
        {
            facing = (facing + 3) % 4;
        }
        else if (step == "R")
        {
            facing = (facing + 1) % 4;
        }
        else if (int.TryParse(step, out int n))
        {
            while (n-- > 0)
            {
                var (nextX, nextY) = facing switch {
                    Right => (currentPos.x + 1, currentPos.y),
                    Down  => (currentPos.x, currentPos.y + 1),
                    Left  => (currentPos.x - 1, currentPos.y),
                    Up    => (currentPos.x, currentPos.y - 1),
                    _     => (currentPos.x, currentPos.y)
                };
                (int nextFacing, nextX, nextY) = transform(facing, nextX, nextY);
                if (IsWall(nextX, nextY)) break;
                currentPos = (nextX, nextY);
                facing = nextFacing;
            }
        }
    }

    return 1000 * (currentPos.y + 1) + 4 * (currentPos.x + 1) + facing;
}

// Part 1
var rowRanges = new (int from, int to)[height];
var colRanges = new (int from, int to)[width];

for (int y = 0; y < height; y++)
{
    rowRanges[y] = (gridLines[y].IndexOfAny(new[] {'.', '#'}), gridLines[y].LastIndexOfAny(new[] {'.', '#'}));
}

for (int x = 0; x < width; x++)
{
    int colRangeFrom = -1, colRangeTo = gridLines.Length - 1;
    for (int y = 0; y < height; y++)
    {
        if (colRangeFrom == -1 && x < gridLines[y].Length && gridLines[y][x] != ' ')
            colRangeFrom = y;
        else if (colRangeFrom >= 0 && x >= gridLines[y].Length)
        {
            colRangeTo = y - 1;
            break;
        }
    }

    colRanges[x] = (colRangeFrom, colRangeTo);
}

(int x, int y) startPos = (rowRanges[0].from, 0);

Console.WriteLine(CalculatePassword(startPos, (int facing, int nextX, int nextY) =>
    facing switch {
        Right => (facing, nextX > rowRanges[nextY].to ? rowRanges[nextY].from : nextX, nextY),
        Down  => (facing, nextX, nextY > colRanges[nextX].to ? colRanges[nextX].from : nextY),
        Left  => (facing, nextX < rowRanges[nextY].from ? rowRanges[nextY].to : nextX, nextY),
        Up    => (facing, nextX, nextY < colRanges[nextX].from ? colRanges[nextX].to : nextY),
        _     => (facing, nextX, nextY)
    }
));

// Part 2

// Detects if you walk over the edge of the current side of the cube, and if so transforms the current coordinates
// and direction according to how the input cube was folded.
(int facing, int x, int y) CubeTransformPosition(int facing, int x, int y)
{
    if (width > 16)
    {
        /*
            Input shape, scale 1:10, hard coded.
            Side naming/numbering: F/1 = Front, R/2 = Right, D/3 = Down, L/4 = Left,  B/5 = Back, U/6 = Up

                   F1F1F  R2R2R
                   1F1F1  2R2R2
                   F1F1F  R2R2R
                   1F1F1  2R2R2
                   F1F1F  R2R2R

                   D3D3D
                   3D3D3
                   D3D3D
                   3D3D3
                   D3D3D

            L4L4L  B5B5B
            4L4L4  5B5B5
            L4L4L  B5B5B
            4L4L4  5B5B5
            L4L4L  B5B5B

            U6U6U
            6U6U6
            U6U6U
            6U6U6
            U6U6U
        */    
        return (facing, x, y) switch
        {
            (Right, 100, >= 0 and < 50)     => (Right, x, y),           // Side 1 -> 2
            (Down, >= 50 and < 100, 50)     => (Down, x, y),            // Side 1 -> 3
            (Left, 49, >= 0 and < 50)       => (Right, 0, 149 - y),     // Side 1 -> 4
            (Up, >= 50 and < 100, -1)       => (Right, 0, 100 + x),     // Side 1 -> 6

            (Right, 150, >= 0 and < 50)     => (Left, 99, 149 - y),     // Side 2 -> 5
            (Down, >= 100 and < 150, 50)    => (Left, 99, x - 50),      // Side 2 -> 3
            (Left, 99, >= 0 and < 50)       => (Left, x, y),            // Side 2 -> 1
            (Up, >= 100 and < 150, -1)      => (Up, x - 100, 199),      // Side 2 -> 6

            (Right, 100, >= 50 and < 100)   => (Up, 50 + y, 49),        // Side 3 -> 2
            (Down, >= 50 and < 100, 100)    => (Down, x, y),            // Side 3 -> 5
            (Left, 49, >= 50 and < 100)     => (Down, y - 50, 100),     // Side 3 -> 4
            (Up, >= 50 and < 100, 49)       => (Up, x, y),              // Side 3 -> 1

            (Right, 50, >= 100 and < 150)   => (Right, x, y),           // Side 4 -> 5
            (Down, >= 0 and < 50, 150)      => (Down, x, y),            // Side 4 -> 6
            (Left, -1, >= 100 and < 150)    => (Right, 50, 149 - y),    // Side 4 -> 1
            (Up, >= 0 and < 50, 99)         => (Right, 50, 50 + x),     // Side 4 -> 3

            (Right, 100, >= 100 and < 150)  => (Left, 149, 149 - y),    // Side 5 -> 2
            (Down, >= 50 and < 100, 150)    => (Left, 49, 100 + x),     // Side 5 -> 6
            (Left, 49, >= 100 and < 150)    => (Left, x, y),            // Side 5 -> 4
            (Up, >= 50 and < 100, 99)       => (Up, x, y),              // Side 5 -> 3

            (Right, 50, >= 150 and < 200)   => (Up, y - 100, 149),      // Side 6 -> 5
            (Down, >= 0 and < 50, 200)      => (Down, 100 + x, 0),      // Side 6 -> 2
            (Left, -1, >= 150 and < 200)    => (Down, y - 100, 0),      // Side 6 -> 1
            (Up, >= 0 and < 50, 149)        => (Up, x, y),              // Side 6 -> 4

            _                               => (facing, x, y)
        };
    }
    else
    {
        /*
            Example input version: F/1 = Front, U/2 = Up, L/3 = Left, D/4 = Down, B/5 = Back, R/6 = Right

                        1F1F
                        F1F1
                        1F1F
                        F1F1

            2U2U  3L3L  4D4D
            U2U2  L3L3  D4D4
            2U2U  3L3L  4D4D
            U2U2  L3L3  D4D4

                        5B5B  6R6R
                        B5B5  R6R6
                        5B5B  6R6R
                        B5B5  R6R6

        */
        return (facing, x, y) switch
        {
            (Right, 12, >= 0 and < 4)       => (Left, 15, 11 - y),      // Side 1 -> 6
            (Down, >= 8 and < 12, 4)        => (Down, x, y),            // Side 1 -> 4
            (Left, 7, >= 0 and < 4)         => (Down, 4 + y, 4),        // Side 1 -> 3
            (Up, >= 8 and < 12, -1)         => (Down, 11 - x, 4),       // Side 1 -> 2

            (Right, 4, >= 4 and < 8)        => (Right, x, y),           // Side 2 -> 3
            (Down, >= 0 and < 4, 8)         => (Up, 11 - x, 11),        // Side 2 -> 5
            (Left, -1, >= 4 and < 8)        => (Up, 19 - y, 11),        // Side 2 -> 6
            (Up, >= 0 and < 4, 3)           => (Down, 11 - x, 0),       // Side 2 -> 1

            (Right, 8, >= 4 and < 8)        => (Right, x, y),           // Side 3 -> 4
            (Down, >= 4 and < 8, 8)         => (Right, 8, 15 - x),      // Side 3 -> 5
            (Left, 3, >= 4 and < 8)         => (Left, x, y),            // Side 3 -> 2
            (Up, >= 4 and < 8, 3)           => (Right, 8, x - 4),       // Side 3 -> 1

            (Right, 12, >= 4 and < 8)       => (Down, 19 - y, 8),       // Side 4 -> 6
            (Down, >= 8 and < 12, 8)        => (Down, x, y),            // Side 4 -> 5
            (Left, 7, >= 4 and < 8)         => (Left, x, y),            // Side 4 -> 3
            (Up, >= 8 and < 12, 3)          => (Up, x, y),              // Side 4 -> 1

            (Right, 12, >= 8 and < 12)      => (Right, x, y),           // Side 5 -> 6
            (Down, >= 8 and < 12, 12)       => (Up, 11 - x, 7),         // Side 5 -> 2
            (Left, 7, >= 8 and < 12)        => (Up, 15 - y, 7),         // Side 5 -> 3
            (Up, >= 8 and < 12, 7)          => (Up, x, y),              // Side 5 -> 4

            (Right, 16, >= 8 and < 12)      => (Left, 11, 11 - y),      // Side 6 -> 1
            (Down, >= 12 and < 16, 12)      => (Right, 0, 19 - x),      // Side 6 -> 2
            (Left, 11, >= 8 and < 12)       => (Left, x, y),            // Side 6 -> 5
            (Up, >= 12 and < 16, 7)         => (Left, 11, 19 - x),      // Side 6 -> 4

            _                               => (facing, x, y)
        };
    }
}

Console.WriteLine(CalculatePassword(startPos, CubeTransformPosition));
