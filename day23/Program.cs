var input = File.ReadAllLines("input.txt");
var directionOffsets = new[] {
    new (int x , int y)[] {(-1, -1), (0, -1), (1, -1)}, // NW, N, NE 
    new (int x , int y)[] {(-1, 1), (0, 1), (1, 1)},    // SW, S, SE
    new (int x , int y)[] {(-1, -1), (-1, 0), (-1, 1)}, // NW, W, SW
    new (int x , int y)[] {(1, -1), (1, 0), (1, 1)},    // NE, E, SE
};
var allDirections = directionOffsets.SelectMany(x => x).Distinct();
var elfPositions = new HashSet<(int x, int y)>();

for (int y = 0; y < input.Length; y++)
{
    for (int x = 0; x < input[y].Length; x++)
    {
        if (input[y][x] == '#') elfPositions.Add((x, y));
    }
}


for (int round = 0; ; round++)
{
    var proposals = new Dictionary<(int x, int y), List<(int x, int y)>>();

    foreach (var elf in elfPositions)
    {
        bool done = false;
        for (int dir = 0; dir < 4 && !done; dir++)
        {
            var effectiveDir = (round + dir) % 4;
            var offsets = directionOffsets[effectiveDir];
            if (allDirections.All(offset => !elfPositions.Contains((elf.x + offset.x, elf.y + offset.y))))
            {
                done = true;
            }
            else if (offsets.All(offset => !elfPositions.Contains((elf.x + offset.x, elf.y + offset.y))))
            {
                var proposedPos = (elf.x + offsets[1].x, elf.y + offsets[1].y);
                List<(int x, int y)>? proposedElves;
                if (!proposals.TryGetValue(proposedPos, out proposedElves))
                {
                    proposedElves = new();
                    proposals.Add(proposedPos, proposedElves);                    
                }
                proposedElves.Add(elf);
                done = true;
            }
        }
    }

    bool anyMoved = false;
    foreach (var proposal in proposals)
    {
        if (proposal.Value.Count == 1)
        {
            var moveFrom = proposal.Value[0];
            var moveTo = proposal.Key;
            elfPositions.Remove(moveFrom);
            elfPositions.Add(moveTo);
            anyMoved = true;
        }
    }

    // Part 1
    if (round == 10)
    {
        int minX = elfPositions.Min(p => p.x),
            maxX = elfPositions.Max(p => p.x),
            minY = elfPositions.Min(p => p.y),
            maxY = elfPositions.Max(p => p.y);
        Console.WriteLine((maxX - minX + 1) * (maxY - minY + 1) - elfPositions.Count);
    }
    // Part 2
    else if (!anyMoved)
    {
        Console.WriteLine(round + 1);
        break;
    }
}
