int Priority(char c) => Char.IsUpper(c) ? c - 'A' + 27 : c - 'a' + 1;

var input = File.ReadAllLines("input.txt");

// Part 1
var sumOfPriorities = input
                        .Select(line => (left:  line[..(line.Length/2)],
                                         right: line[(line.Length/2)..]))
                        .Sum(rucksack =>
                            Priority(rucksack.left
                                        .Intersect(rucksack.right)
                                        .Single())
                        );
Console.WriteLine(sumOfPriorities);

// Part 2
var groupSum = input
                .Chunk(3)
                .Sum(group =>
                    Priority(group[0]
                                .Intersect(group[1])
                                .Intersect(group[2])
                                .Single())
                );
Console.WriteLine(groupSum);
