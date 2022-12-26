
long Solve(IEnumerable<long> inputList, int key, int rounds)
{
    var list = new LinkedList<long>(inputList.Select(i => i * key));
    var moveQueue = new Queue<LinkedListNode<long>>();
    LinkedListNode<long>? zero = default;

    while (rounds-- > 0)
    {
        var node = list.First;
        while (node != null)
        {
            moveQueue.Enqueue(node);
            node = node.Next;
        }
    }

    while (moveQueue.TryDequeue(out var numberToMove))
    {
        long steps = numberToMove.Value % (list.Count - 1);

        if (numberToMove.Value == 0) {
            zero = numberToMove;
            continue;
        }
        else if (steps < 0)
        {
            var addBefore = numberToMove;
            while (steps++ < 0)
            {
                addBefore = addBefore.Previous ?? list.Last!;
            }
            list.Remove(numberToMove);
            list.AddBefore(addBefore, numberToMove);
        }
        else
        {
            var addAfter = numberToMove;
            while (steps-- > 0)
            {
                addAfter = addAfter.Next ?? list.First!;
            }
            list.Remove(numberToMove);
            list.AddAfter(addAfter, numberToMove);
        }
    }

    long sum = 0;
    if (zero != null)
    {
        var node = zero;
        for (int i = 1; i <= 3000; i++)
        {
            node = node.Next ?? list.First!;
            if (i % 1000 == 0)
            {
                sum += node.Value;
            }
        }
    }

    return sum;
}

var input = File.ReadAllLines("input.txt").Select(long.Parse);

long part1 = Solve(input, 1, 1);
Console.WriteLine(part1);

long part2 = Solve(input, 811589153, 10);
Console.WriteLine(part2);
