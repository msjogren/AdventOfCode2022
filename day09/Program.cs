int Solve(string[] moves, int knotCount)
{
    var startPos = (x: 0, y: 0);
    var knots = Enumerable.Repeat(startPos, knotCount).ToArray();
    var tailVisited = new HashSet<(int x, int y)>() { startPos };

    foreach (var line in moves)
    {
        int moveSteps = int.Parse(line[2..]);
        var delta = line[0] switch {
            'R' => (x:  1, y:  0),
            'L' => (x: -1, y:  0),
            'U' => (x:  0, y: -1),
            'D' => (x:  0, y:  1),
            _ => throw new InvalidOperationException()
        };

        while (moveSteps-- > 0)
        {
            knots[0] = (knots[0].x + delta.x, knots[0].y + delta.y);
            for (int i = 1; i < knotCount; i++)
            {
                if (Math.Abs(knots[i - 1].x - knots[i].x) > 1 || Math.Abs(knots[i - 1].y - knots[i].y) > 1)
                {
                    knots[i] = (knots[i].x + Math.Sign(knots[i - 1].x - knots[i].x), knots[i].y + Math.Sign(knots[i - 1].y - knots[i].y));
                    if (i == knotCount - 1) tailVisited.Add(knots[i]);
                }
            } 
        }
    }

    return tailVisited.Count();
}

var input = File.ReadAllLines("input.txt");

// Part 1
Console.WriteLine(Solve(input, 2));

// Part 2
Console.WriteLine(Solve(input, 10));
