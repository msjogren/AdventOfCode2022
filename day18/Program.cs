var input = File.ReadAllLines("input.txt");
var cubes = input.Select(line => {
    var coords = line.Split(',').Select(int.Parse).ToArray();
    return (x: coords[0], y: coords[1], z: coords[2]);
}).ToList();

// Part 1
int ManhattanDistance((int x, int y, int z) pt1, (int x, int y, int z) pt2)
{
    return Math.Abs(pt2.x - pt1.x) + Math.Abs(pt2.y - pt1.y) + Math.Abs(pt2.z - pt1.z);
}

var exposedSides = cubes.ToDictionary(cubeCoords => cubeCoords, _ => 6);

for (int i = 0; i < cubes.Count - 1; i++)
{
    for (int j = i + 1; j < cubes.Count; j++)
    {
        if (ManhattanDistance(cubes[i], cubes[j]) == 1)
        {
            exposedSides[cubes[i]]--;
            exposedSides[cubes[j]]--;
        }
    }
}

Console.WriteLine(exposedSides.Values.Sum());


// Part 2
int minX = cubes.Select(c => c.x).Min() - 1,
    maxX = cubes.Select(c => c.x).Max() + 1,
    minY = cubes.Select(c => c.y).Min() - 1,
    maxY = cubes.Select(c => c.y).Max() + 1,
    minZ = cubes.Select(c => c.z).Min() - 1,
    maxZ = cubes.Select(c => c.z).Max() + 1;

var steamOrigin = (x: minX, y: minY, z:minZ);
var steamQueue = new Queue<(int x, int y, int z)>();
var steamSeen = new HashSet<(int x, int y, int z)>();
int exteriorSides = 0;
var adjacentOffsets = new (int x, int y, int z)[] {(0, 0, -1), (0, 0, 1), (0, -1, 0), (0, 1, 0), (-1, 0, 0), (1, 0, 0)};

steamQueue.Enqueue(steamOrigin);
while (steamQueue.TryDequeue(out var currentSteam))
{
    if (steamSeen.Contains(currentSteam)) continue;
    steamSeen.Add(currentSteam);

    foreach (var offset in adjacentOffsets)
    {
        var adjacent = (x: currentSteam.x + offset.x, y: currentSteam.y + offset.y, z: currentSteam.z + offset.z);
        if (adjacent.x < minX || adjacent.x > maxX ||
            adjacent.y < minY || adjacent.y > maxY ||
            adjacent.z < minZ || adjacent.z > maxZ) continue;

        if (cubes.Contains(adjacent))
            exteriorSides++;            
        else
            steamQueue.Enqueue(adjacent);
    }
}

Console.WriteLine(exteriorSides);
