
var input = File.ReadAllLines("input.txt").Select(line => {
    var ranges = line.Split(new[] {',', '-'}).Select(int.Parse).ToArray();
    return (from1: ranges[0], to1: ranges[1], from2: ranges[2], to2: ranges[3]);    
});

// Part 1
int contained = input.Count(r => (r.from1 >= r.from2 && r.to1 <= r.to2) ||
                                 (r.from2 >= r.from1 && r.to2 <= r.to1));
Console.WriteLine(contained);

// Part 2
int overlapped = input.Count() - input.Count(r => r.from2 > r.to1 || r.from1 > r.to2);
Console.WriteLine(overlapped);
