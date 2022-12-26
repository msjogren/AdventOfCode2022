var input = File.ReadAllLines("input.txt");
var keyedInput = input.Select(line => {
    var parts = line.Split(": ");
    return (key: parts[0], expr: parts[1]);
}).ToDictionary(i => i.key, i => i.expr);

MonkeyJob BuildExpressionTree(IDictionary<string, string> inputExpressions, string monkeyName, string humanName = "")
{
    if (monkeyName == humanName)
    {
        return new MonkeyJob(monkeyName);
    }
    else if (long.TryParse(inputExpressions[monkeyName], out long n))
    {
        return new MonkeyJob(monkeyName, n);
    }
    else
    {
        var operationParts = inputExpressions[monkeyName].Split(' ');
        var left = BuildExpressionTree(inputExpressions, operationParts[0], humanName);
        var right = BuildExpressionTree(inputExpressions, operationParts[2], humanName);
        return new MonkeyJob(monkeyName, left, right, operationParts[1][0]);
    }
}

const string Root = "root";
const string Human = "humn";

// Part 1
var part1Root = BuildExpressionTree(keyedInput, Root);
part1Root.Evaluate();
Console.WriteLine(part1Root.KnownValue);

// Part 2
var part2Root = BuildExpressionTree(keyedInput, Root, Human);
part2Root.Evaluate();

long equationResult = 0;
MonkeyJob? equationExpression = default;
if (part2Root.Left?.KnownValue is null && part2Root.Right?.KnownValue is long r)
{
    equationExpression = part2Root.Left;
    equationResult = r;
}
else if (part2Root.Right?.KnownValue is null && part2Root.Left?.KnownValue is long l)
{
    equationExpression = part2Root.Right;
    equationResult = l;
}

while (equationExpression is MonkeyJob && equationExpression.Name != Human)
{
    var reverseOperator = equationExpression.Operator;
    if (equationExpression.Left?.KnownValue is long lhs)
    {
        equationResult = reverseOperator switch {
            '+' => equationResult - lhs,
            '-' => lhs - equationResult,
            '*' => equationResult / lhs,
            '/' => lhs / equationResult,
            _ => throw new InvalidOperationException()
        };
        equationExpression = equationExpression.Right;
    }
    else if (equationExpression.Right?.KnownValue is long rhs)
    {
        equationResult = reverseOperator switch {
            '+' => equationResult - rhs,
            '-' => equationResult + rhs,
            '*' => equationResult / rhs,
            '/' => equationResult * rhs,
            _ => throw new InvalidOperationException()
        };
        equationExpression = equationExpression.Left;
    }
}

Console.WriteLine(equationResult);

class MonkeyJob
{
    public string Name;
    public MonkeyJob? Left;
    public MonkeyJob? Right;
    public char Operator;
    public long? KnownValue;

    public MonkeyJob(string name)
    {
        Name = name;
    }

    public MonkeyJob(string name, long knownValue)
    {
        Name = name;
        KnownValue = knownValue;
    }

    public MonkeyJob(string name, MonkeyJob left, MonkeyJob right, char op)
    {
        Name = name;
        Left = left;
        Right = right;
        Operator = op;
    }

    public bool Evaluate()
    {
        if (KnownValue.HasValue) return false;

        bool subChanges = (Left?.Evaluate() | Right?.Evaluate()) ?? false;

        if (Left?.KnownValue is long lhs && Right?.KnownValue is long rhs)
        {
            KnownValue = Operator switch {
                '+' => lhs + rhs,
                '-' => lhs - rhs,
                '*' => lhs * rhs,
                '/' => lhs / rhs,
                _ => throw new InvalidOperationException()
            };
            return true;
        }

        return subChanges;
    }
}