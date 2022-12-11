var input = File.ReadAllLines("input.txt");
var instructions = new Queue<string>(input);

int x = 1;
int? addX = null;
int signalStrenghs = 0;

for (int cycle = 1; instructions.Any(); cycle++)
{
    if ((cycle - 20) % 40 == 0)
    {
        signalStrenghs += x * cycle;
    }

    int crtPixelPos = (cycle - 1) % 40;
    bool lit = crtPixelPos >= x - 1 && crtPixelPos <= x + 1;
    if (crtPixelPos == 0)
    {
        Console.WriteLine();
    }
    Console.Write(lit ? '#' : '.');

    if (addX != null)
    {
        x += addX.Value;
        addX = null;
    }
    else
    {
        var instr = instructions.Dequeue();
        if (instr.StartsWith("addx "))
        {
            addX = int.Parse(instr[5..]);
        }
    }
}

Console.WriteLine();
Console.WriteLine(signalStrenghs);