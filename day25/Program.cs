
long SnafuToDecimal(string snafu)
{
    long power = 1, result = 0;
    foreach (char c in snafu.Reverse())
    {
        result += power * (c switch {
            '2' => 2,
            '1' => 1,
            '0' => 0,
            '-' => -1,
            '=' => -2,
            _ => throw new ArgumentOutOfRangeException(nameof(snafu))
        });
        power *= 5;
    }
    return result;
}

string DecimalToSnafu(long dec)
{
    List<char> result = new();
    int carryOver = 0;

    while (dec > 0 || carryOver > 0)
    {
        (carryOver, char snafuDigit) = ((dec + carryOver) % 5) switch
        {
            0 => (0, '0'),
            1 => (0, '1'),
            2 => (0, '2'),
            3 => (1, '='),
            4 => (1, '-'),
            _ => throw new ArgumentOutOfRangeException(nameof(dec))
        };
        result.Insert(0, snafuDigit);
        dec /= 5;
    }

    return new string(result.ToArray());
}

var input = File.ReadAllLines("input.txt");
long inputSum = input.Sum(line => SnafuToDecimal(line));
Console.WriteLine(DecimalToSnafu(inputSum));
