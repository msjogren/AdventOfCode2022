var input = File.ReadAllLines("input.txt");
Directory root = new Directory(), current = root;
var allDirectories = new List<Directory> { root };

foreach (string line in input)
{
    if (line.StartsWith("$ cd "))
    {
        switch (line[5..])
        {
            case "/":
                continue;
            case "..":
                current = current.Parent!;
                break;
            default:
                current = new Directory() { Parent = current };
                allDirectories.Add(current);
                break;
        }
    }
    else if (Char.IsNumber(line[0]))
    {
        current.AddFileSize(int.Parse(line.Substring(0, line.IndexOf(' '))));
    }
}

// Part 1
Console.WriteLine(allDirectories.Where(d => d.TotalSize < 100_000).Sum(d => d.TotalSize));

// Part 2
const int DiskSize = 70_000_000;
const int SpaceRequired = 30_000_000;
int spaceToFree = root.TotalSize - (DiskSize - SpaceRequired);
Console.WriteLine(allDirectories.Where(d => d.TotalSize >= spaceToFree).Min(d => d.TotalSize));

class Directory
{
    public Directory? Parent { get; init; }
    public int TotalSize { get; private set; }
    public void AddFileSize(int size)
    {
        TotalSize += size;
        Parent?.AddFileSize(size);
    }
}
