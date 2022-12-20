var input = File.ReadAllLines("input.txt");

var sensorData = input.Select(line => {
    (int x, int y) ParseCoords(string s) {
        int xpos = s.IndexOf('x');
        int ypos = s.IndexOf('y', xpos);
        int x = int.Parse(s[(xpos+2)..(ypos-2)]);
        int y = int.Parse(s[(ypos+2)..]);
        return (x, y);
    }
    var parts = line.Split(':');
    var sensorPos = ParseCoords(parts[0]);
    var beaconPos = ParseCoords(parts[1]);
    return (sensor: sensorPos, beacon: beaconPos, r: ManhattanDistance(sensorPos, beaconPos));
}).ToArray();

int ManhattanDistance((int x, int y) p1, (int x, int y) p2)
    => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);

// Part 1
const int Row = 2_000_000;
var occupied = new HashSet<int>();  
var covered = new HashSet<int>();

foreach (var sd in sensorData)
{
    if (sd.beacon.y == Row) occupied.Add(sd.beacon.x);
    if (sd.sensor.y == Row) occupied.Add(sd.sensor.x);

    if (Math.Abs(sd.sensor.y - Row) > sd.r)
        continue;

    for (int x = sd.sensor.x - sd.r; x <= sd.sensor.x + sd.r; x++)
    {
        if (ManhattanDistance(sd.sensor, (x, Row)) <= sd.r)
            covered.Add(x);
    }
}

covered.ExceptWith(occupied);
Console.WriteLine(covered.Count);

// Part 2
const int Width = 4_000_000, Height = 4_000_000;
var pointsOutsideRange = new HashSet<(int x, int y)>();

for (int i = 0; i < sensorData.Length - 1; i++)
{
    var sd = sensorData[i];
    int outsideDistance = sd.r + 1;

    for (int x = -outsideDistance; x <= outsideDistance; x++)
    {
        void ConsiderAdding((int x, int y) pt, HashSet<(int, int)> points)
        {
            if (pt.x < 0 && pt.x >= Width && pt.y < 0 && pt.y >= Height) return;

            for (int j = i + 1; j < sensorData.Length; j++)
            {
                if (ManhattanDistance(sensorData[j].sensor, pt) == sensorData[j].r + 1)
                {
                    points.Add(pt);
                }
            }
        }

        ConsiderAdding((sd.sensor.x + x, sd.sensor.y + (outsideDistance - Math.Abs(x))), pointsOutsideRange);
        ConsiderAdding((sd.sensor.x + x, sd.sensor.y - (outsideDistance - Math.Abs(x))), pointsOutsideRange);
    }
}

var uncoveredPoint = pointsOutsideRange.First(pt => !sensorData.Any(sd => ManhattanDistance(pt, sd.sensor) <= sd.r));
Console.WriteLine(4_000_000L * uncoveredPoint.x + uncoveredPoint.y);
