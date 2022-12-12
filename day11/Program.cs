Monkey[] ParseInput(string[] lines)
{
    var inputPerMonkey = lines.Chunk(7);
    var monkeys = new Monkey[inputPerMonkey.Count()];
    int monkeyCount = 0;

    foreach (var monkeyData in inputPerMonkey)
    {
        var items = monkeyData[1].Substring(monkeyData[1].IndexOf(':') + 2).Split(", ").Select(long.Parse);
        var operation = monkeyData[2].Substring(monkeyData[2].IndexOf("old ") + 4).Split(' ');

        monkeys[monkeyCount++] = new Monkey() {
            Items = new Queue<long>(items),
            Operator = operation[0][0],
            Operand = operation[1],
            Divisor = long.Parse(monkeyData[3].Substring(monkeyData[3].LastIndexOf(' ') + 1)),
            TrueTarget = int.Parse(monkeyData[4].Substring(monkeyData[4].LastIndexOf(' ') + 1)),
            FalseTarget = int.Parse(monkeyData[5].Substring(monkeyData[5].LastIndexOf(' ') + 1)),
        };
    }

    return monkeys;
}

long CalculateMonkeyBusiness(Monkey[] monkeys, int rounds, Func<long, long> relieve)
{
    while (rounds-- > 0)
    {
        foreach (var currentMonkey in monkeys)
        {
            while (currentMonkey.Items.TryDequeue(out long item))
            {
                currentMonkey.Inspections++;

                long operand = currentMonkey.Operand == "old" ? item : long.Parse(currentMonkey.Operand);
                long newItem = relieve(currentMonkey.Operator == '+' ? item + operand : item * operand);
                int target = newItem % currentMonkey.Divisor == 0 ? currentMonkey.TrueTarget : currentMonkey.FalseTarget;
                monkeys[target].Items.Enqueue(newItem);
            }
        }
    }

    return monkeys.OrderByDescending(m => m.Inspections).Take(2).Select(m => m.Inspections).Multiply();
}

var input = File.ReadAllLines("input.txt");

// Part 1
Console.WriteLine(CalculateMonkeyBusiness(ParseInput(input), 20, n => n / 3));

// Part 2
var part2Monkeys = ParseInput(input);
long part2CommonMultiplier = part2Monkeys.Select(m => m.Divisor).Multiply();
Console.WriteLine(CalculateMonkeyBusiness(part2Monkeys, 10_000, n => n % part2CommonMultiplier));

class Monkey
{
    public long Inspections { get; set; }
    public Queue<long> Items { get; init; }
    public long Divisor { get; init; }
    public char Operator { get; init; }
    public string Operand { get; init; }
    public int TrueTarget { get; init; }
    public int FalseTarget { get; init; }
}

static class EnumerableExtensions
{
    public static long Multiply(this IEnumerable<long> enumerable) => enumerable.Aggregate(1L, (a, b) => a * b);
}