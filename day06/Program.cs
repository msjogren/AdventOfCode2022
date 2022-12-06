bool IsMarker(ReadOnlySpan<char> str)
{
    int n = str.Length;
    for (int i = 0; i < n - 1; i++)
    {
        char ch = str[i];
        for (int j = i + 1; j < n; j++)
        {
            if (str[j] == ch) return false;
        }
    }

    return true;
}

int FindMarkerOfLength(string buffer, int length)
{
    for (int i = length; i < buffer.Length; i++)
    {
        if (IsMarker(buffer.AsSpan()[(i-length)..i]))
        {
            return i;
        }
    }

    return -1;
}


string input = File.ReadAllText("input.txt");

// Part 1
int FindStartOfPacketMarker(string buffer) => FindMarkerOfLength(buffer, 4);
Console.WriteLine(FindStartOfPacketMarker(input));

// Part 2
int FindStartOfMessageMarker(string buffer) => FindMarkerOfLength(buffer, 14);
Console.WriteLine(FindStartOfMessageMarker(input));
