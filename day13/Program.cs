var input = File.ReadAllLines("input.txt");
var comparer = new PacketComparer();

// Part 1
var sum = input
    .Chunk(3)
    .Select((packetPair, index) => (index, packetPair))
    .Where(indexedPackets => comparer.Compare(indexedPackets.packetPair[0], indexedPackets.packetPair[1]) < 0)
    .Sum(packets => packets.index + 1);
Console.WriteLine(sum);

// Part 2
const string DividerPacket1 = "[[2]]",
             DividerPacket2 = "[[6]]";
var decoderKey = input
    .Where(line => line != "")
    .Append(DividerPacket1)
    .Append(DividerPacket2)
    .OrderBy(s => s, comparer)
    .Select((packet, index) => (index, packet))
    .Where(indexedPacked => indexedPacked.packet == DividerPacket1 || indexedPacked.packet == DividerPacket2)
    .Select(indexedPacket => indexedPacket.index + 1)
    .Aggregate(1, (a, b) => a * b);
Console.WriteLine(decoderKey);



class PacketComparer : IComparer<string>
{    
    public int Compare(string? x, string? y)
    {
        return (x, y) switch {
            (null, null) => 0,
            (null, _) => -1,
            (_, null) => 1,
            _ => CompareLists(ParseList(x), ParseList(y))
        };
    }

    List<object> ParseList(string list)
    {
        int pos = 0;
        return ParseList(list, ref pos);
    }

    List<object> ParseList(string list, ref int pos)
    {
        if (list[pos++] != '[') throw new ArgumentException("Not a list");

        var result = new List<object>();
        while (true)
        {
            switch (list[pos])
            {
                case ']':
                    pos++;
                    return result;

                case '[':
                    result.Add(ParseList(list, ref pos));
                    break;

                case ',':
                    pos++;
                    break;

                default:
                    int startOfNumber = pos;
                    while (char.IsDigit(list[pos])) pos++;
                    result.Add(int.Parse(list[startOfNumber..pos]));
                    break;
            }
        }
    }

    int CompareLists(List<object> leftList, List<object> rightList)
    {
        int leftIdx = 0, rightIdx = 0;

        while (leftIdx < leftList.Count && rightIdx < rightList.Count)
        {
            var result = (leftList[leftIdx++], rightList[rightIdx++]) switch {
                (int leftInt, int rightInt) => leftInt.CompareTo(rightInt),
                (List<object> leftSublist, List<object> rightSublist) => CompareLists(leftSublist, rightSublist),
                (int leftInt, List<object> rightSublist) => CompareLists(new List<object>() { leftInt }, rightSublist),
                (List<object> leftSublist, int rightInt) => CompareLists(leftSublist, new List<object>() { rightInt }),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (result != 0) return result;
        }

        if (leftIdx == leftList.Count && rightIdx == rightList.Count)
            return 0;
        else if (leftIdx == leftList.Count)
            return -1;
        else if (rightIdx == rightList.Count)
            return 1;
        else
            return 0;
    }
}