var elfCalories = File.ReadAllText("input.txt")
                      .Split("\r\n\r\n")
                      .Select(elf => 
                        elf.Split("\r\n")
                           .Sum(int.Parse)
                      );

// Part 1
Console.WriteLine(elfCalories.Max());

// Part 2
Console.WriteLine(elfCalories.OrderByDescending(c => c).Take(3).Sum());
