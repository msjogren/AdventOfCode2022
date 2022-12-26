var input = File.ReadAllText("input.txt");

const int ChamberWidth = 7;
const long Part2Rocks = 1_000_000_000_000;
var chamber = new bool[ChamberWidth, 10_000];


IEnumerable<IList<(int x, int y)>> EnumerateRocks()
{
    for (int rockIndex = 0; ; rockIndex++)
    {
        yield return (rockIndex % 5) switch
        {
            // ####
            0 => new[] {(0, 0), (1, 0), (2, 0), (3, 0)},
            // .#.
            // ###
            // .#.
            1 => new[] {(1, 0), (0, 1), (1, 1), (2, 1), (1, 2)},
            // ..#
            // ..#
            // ###
            2 => new[] {(0, 0), (1, 0), (2, 0), (2, 1), (2, 2)},
            // #
            // #
            // #
            // #
            3 => new[] {(0, 0), (0, 1), (0, 2), (0, 3)},
            // ##
            // ##
            4 => new[] {(0, 0), (1, 0), (0, 1), (1, 1)},
        };
    }
}


var seenStates = new Dictionary<(int rockType, int jetIndex), (long rockCount, long towerHeight)>();
long rockCount = 0;
int jetIndex = 0;
long towerHeight = 0;
long part2TowerHeight = 0;
bool searchForCycle = false;
long lastCycleLength = 0;

foreach (var rockShape in EnumerateRocks())
{
    int rockType = (int)(rockCount++ % 5);
    int rockWidth = rockShape.Max(pt => pt.x) + 1,
        rockHeight = rockShape.Max(pt => pt.y) + 1;    
    int rockPosX = 2;
    long rockPosY = towerHeight + 3;
    bool atRest = false;

    while (!atRest)
    {
        const int dy = -1;
        int dx = input[jetIndex] == '<' ? -1 : 1;
        jetIndex = (jetIndex + 1) % input.Length;

        if (rockPosX + dx >= 0 && (rockPosX + dx + rockWidth) <= ChamberWidth && rockShape.All(r => chamber[rockPosX + dx + r.x, rockPosY + r.y] == false))
                rockPosX += dx;

        if (rockPosY > 0 && rockShape.All(r => chamber[rockPosX + r.x, rockPosY + r.y + dy] == false))
        {
            rockPosY += dy;
        }
        else
        {
            atRest = true;
            towerHeight = Math.Max(towerHeight, rockPosY + rockHeight);
            foreach (var pt in rockShape)
            {
                chamber[rockPosX + pt.x, rockPosY + pt.y] = true;
            }

            if (rockCount == 2022)
            {
                Console.WriteLine(towerHeight);
                searchForCycle = true;
            }

            if (searchForCycle)
            {
                var state = (rockType, jetIndex);
                if (seenStates.TryGetValue(state, out var prev))
                {
                    long cycleLength = rockCount - prev.rockCount;
                    if (cycleLength == lastCycleLength)
                    {
                        searchForCycle = false;
                        //Console.WriteLine($"Recurring cycle found. Rock type {state.rockType}, jet {state.jetIndex} {input[state.jetIndex]}, repeated every {rockCount - prev.rockCount} rocks, increases height by {towerHeight - prev.towerHeight}");

                        var heightCycle = towerHeight - prev.towerHeight;
                        var rockCycle = rockCount - prev.rockCount;
                        var fastForwardCycles = (Part2Rocks - rockCount) / rockCycle;
                        part2TowerHeight = fastForwardCycles * heightCycle;
                        rockCount += fastForwardCycles * rockCycle;
                    }
                    else
                    {
                        lastCycleLength = cycleLength;
                    }
                }
                else
                {
                    seenStates[state] = (rockCount, towerHeight);
                }
            }
        }      
    }

    if (rockCount >= Part2Rocks)
    {
        Console.WriteLine(towerHeight + part2TowerHeight);
        break;
    }
}
