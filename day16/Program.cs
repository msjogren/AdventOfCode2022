var input = File.ReadAllLines("input.txt");

var valveData = input.Select((line, index) => {
    var parts = line.Split(new[] {' ', ';', '=', ','}, StringSplitOptions.RemoveEmptyEntries);
    return (name: parts[1], mask: 1L << index, flowRate: int.Parse(parts[5]), tunnels: parts[10..]);
}).ToArray();

int FindDistance(string fromValve, string toValve)
{
    var visited = new HashSet<string>();
    var queue = new Queue<(string valve, int distance)>();
    queue.Enqueue((fromValve, 0));

    while (queue.TryDequeue(out var currentState))
    {
        if (currentState.valve == toValve)
            return currentState.distance;
        if (visited.Contains(currentState.valve))
            continue;
        visited.Add(currentState.valve);

        foreach (string next in valveData.First(vd => vd.name == currentState.valve).tunnels)
        {
            if (visited.Contains(next)) continue;
            queue.Enqueue((next, currentState.distance + 1));
        }
    }

    return -1;
}

const string startValve = "AA";
long valvesWithFlowMask = valveData.Where(vd => vd.flowRate > 0).Aggregate(0L, (mask, vd) => mask | vd.mask);
var valveDistances = new Dictionary<(string from, string to), int>();

foreach (var from in valveData)
{
    if (from.flowRate == 0 && from.name != startValve) continue;

    foreach (var to in valveData)
    {
        if (from.name == to.name || (to.flowRate == 0 && to.name != startValve)) continue;

        int d = FindDistance(from.name, to.name);
        valveDistances.Add((from.name, to.name), d);
    }
}

void SearchAllValvePaths(string currentValveName, int timeLeft, int releasedPressure, long openedValvesMask, Dictionary<long, int> maxPressureByState)
{
    maxPressureByState[openedValvesMask] = Math.Max(maxPressureByState.GetValueOrDefault(openedValvesMask, 0), releasedPressure);

    long remainingValvesMask = valvesWithFlowMask & ~openedValvesMask;

    foreach (var nextValve in valveData)
    {
        if ((remainingValvesMask & nextValve.mask) == 0) continue;

        int nextTimeLeft = timeLeft - (valveDistances![(currentValveName, nextValve.name)] + 1);
        long nextOpenedValvesMask = openedValvesMask | nextValve.mask;

        if (nextTimeLeft < 0 || nextOpenedValvesMask == valvesWithFlowMask) continue;

        SearchAllValvePaths(
            nextValve.name,
            nextTimeLeft,
            releasedPressure + nextTimeLeft * nextValve.flowRate,
            nextOpenedValvesMask,
            maxPressureByState);
    }
}

// Part 1
var maxPressureByValvesOpened = new Dictionary<long, int>();
SearchAllValvePaths(startValve, 30, 0, 0, maxPressureByValvesOpened);
Console.WriteLine(maxPressureByValvesOpened.Values.Max());

// Part 2
maxPressureByValvesOpened.Clear();
SearchAllValvePaths(startValve, 26, 0, 0, maxPressureByValvesOpened);

int part2Max = 0;
foreach (var myPath in maxPressureByValvesOpened)
{
    foreach (var elephantPath in maxPressureByValvesOpened)
    {
        if ((myPath.Key & elephantPath.Key) != 0) continue;
        part2Max = Math.Max(part2Max, myPath.Value + elephantPath.Value);
    }
}

Console.WriteLine(part2Max);
