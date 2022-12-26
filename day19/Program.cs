var input = File.ReadAllLines("input.txt");

var blueprints = input.Select(line => {
    var costs = line.Split(new[] { "costs ", "and " }, StringSplitOptions.None);

    int GetCost(string s)
    {
        int space = s.IndexOf(' ');
        return int.Parse(s[..space]);
    }

    return new Blueprint()
    {
        OreRobotOreCost = GetCost(costs[1]),
        ClayRobotOreCost = GetCost(costs[2]),
        ObsidianRobotOreCost = GetCost(costs[3]),
        ObsidianRobotClayCost = GetCost(costs[4]),
        GeodeRobotOreCost = GetCost(costs[5]),
        GeodeRobotObsidianCost = GetCost(costs[6]),
    };
}).ToList();


int BfsMaxGeodes(Blueprint blueprint, int minutes)
{
    var seen = new HashSet<State>();
    var queue = new Queue<State>();
    var mostGeodeAtTime = new Dictionary<int, int>();
    int maxOreRequired = new[] {blueprint.OreRobotOreCost, blueprint.ClayRobotOreCost, blueprint.ObsidianRobotOreCost, blueprint.GeodeRobotOreCost}.Max();
    queue.Enqueue(new State(minutes, 0, 0, 0, 0, 1, 0, 0, 0));

    while (queue.TryDequeue(out var s))
    {
        if (seen.Contains(s)) continue;
        seen.Add(s);

        mostGeodeAtTime[s.TimeLeft] = Math.Max(mostGeodeAtTime.GetValueOrDefault(s.TimeLeft, 0), s.Geodes);
        if (s.TimeLeft == 0 || mostGeodeAtTime[s.TimeLeft] > (s.Geodes + s.TimeLeft * s.GeodeRobots)) continue;

        var ore = s.Ore + s.OreRobots;
        var clay = s.Clay + s.ClayRobots;
        var obsidian = s.Obsidian + s.ObsidianRobots;
        var geodes = s.Geodes + s.GeodeRobots;

        if (s.Ore >= blueprint.GeodeRobotOreCost && s.Obsidian >= blueprint.GeodeRobotObsidianCost && s.TimeLeft > 1)
        {
            queue.Enqueue(new State(s.TimeLeft - 1, ore - blueprint.GeodeRobotOreCost, clay, obsidian - blueprint.GeodeRobotObsidianCost, geodes, s.OreRobots, s.ClayRobots, s.ObsidianRobots, s.GeodeRobots + 1));
            continue;
        }
        else
        {
            if (s.Ore >= blueprint.ObsidianRobotOreCost && s.Clay >= blueprint.ObsidianRobotClayCost && s.ObsidianRobots < blueprint.GeodeRobotObsidianCost && s.TimeLeft > 2)
            {
                queue.Enqueue(new State(s.TimeLeft - 1, ore - blueprint.ObsidianRobotOreCost, clay - blueprint.ObsidianRobotClayCost, obsidian, geodes, s.OreRobots, s.ClayRobots, s.ObsidianRobots + 1, s.GeodeRobots));
            }

            if (s.Ore >= blueprint.ClayRobotOreCost && s.ClayRobots < blueprint.ObsidianRobotClayCost && s.TimeLeft > 2)
            {
                queue.Enqueue(new State(s.TimeLeft - 1, ore - blueprint.ClayRobotOreCost, clay, obsidian, geodes, s.OreRobots, s.ClayRobots + 1, s.ObsidianRobots, s.GeodeRobots));
            }

            if (s.Ore >= blueprint.OreRobotOreCost && s.OreRobots < maxOreRequired && s.TimeLeft > 2)
            {
                queue.Enqueue(new State(s.TimeLeft - 1, ore - blueprint.OreRobotOreCost, clay, obsidian, geodes, s.OreRobots + 1, s.ClayRobots, s.ObsidianRobots, s.GeodeRobots));
            }

            queue.Enqueue(new State(s.TimeLeft - 1, ore, clay, obsidian, geodes, s.OreRobots, s.ClayRobots, s.ObsidianRobots, s.GeodeRobots));
        }
    }

    return mostGeodeAtTime[0];
}


// Part 1
int qualityLevel = blueprints.Select((blueprint, index) => BfsMaxGeodes(blueprint, 24) * (index + 1)).Sum();
Console.WriteLine(qualityLevel);

// Part 2
int part2 = blueprints.Take(3).Select(blueprint => BfsMaxGeodes(blueprint, 32)).Aggregate(1, (a, b) => a * b);
Console.WriteLine(part2);

record State(int TimeLeft, int Ore, int Clay, int Obsidian, int Geodes, int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots);

class Blueprint
{
    public int OreRobotOreCost;
    public int ClayRobotOreCost;
    public int ObsidianRobotOreCost;
    public int ObsidianRobotClayCost;
    public int GeodeRobotOreCost;
    public int GeodeRobotObsidianCost;
}
