var input = File.ReadAllLines("input.txt");
int height = input.Length, width = input[0].Length;
int[,] grid = new int[width, height];

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        grid[x, y] = input[y][x] - '0';
    }
}

// Part 1
int AddIfVisible(int currentTallest, int x, int y, HashSet<(int, int)> set)
{
    if (grid[x, y] <= currentTallest) return currentTallest;
    set.Add((x, y));
    return grid[x, y];
}

var allVisible = new HashSet<(int x, int y)>();

for (int treeY = 0; treeY < height; treeY++)
{
    // Left to right
    int tallest = -1;
    for (int treeX = 0; treeX < width && tallest < 9; treeX++)
        tallest = AddIfVisible(tallest, treeX, treeY, allVisible);
 
    // Right to left
    tallest = -1;
    for (int treeX = width - 1; treeX >= 0 && tallest < 9; treeX--)
        tallest = AddIfVisible(tallest, treeX, treeY, allVisible);
}

for (int treeX = 1; treeX < width - 1; treeX++)
{
    // Top to bottom
    int tallest = -1;
    for (int treeY = 0; treeY < height && tallest < 9; treeY++)
        tallest = AddIfVisible(tallest, treeX, treeY, allVisible);

    // Bottom to top
    tallest = -1;
    for (int treeY = height - 1; treeY >= 0 && tallest < 9; treeY--)
        tallest = AddIfVisible(tallest, treeX, treeY, allVisible);
}

Console.WriteLine(allVisible.Count());


// Part 2
int bestScenicScore = 0;
for (int treeY = 1; treeY < height - 1; treeY++)
{
    for (int treeX = 1; treeX < width - 1; treeX++)
    {  
        int GetViewingDistance(int deltaX, int deltaY)
        {
            int treeHeight = grid[treeX, treeY];
            int x = treeX, y = treeY;
            int distance = 0;
            do  {
                distance++;
                x += deltaX;
                y += deltaY; 
            } while (x > 0 && x < width - 1 && y > 0 && y < height - 1 && grid[x, y] < treeHeight);
            return distance;
        }

        int scenicScore = GetViewingDistance( 0, -1) *
                          GetViewingDistance( 0,  1) *
                          GetViewingDistance(-1,  0) *
                          GetViewingDistance( 1,  0);
        if (scenicScore > bestScenicScore) bestScenicScore = scenicScore;
    }
}

Console.WriteLine(bestScenicScore);
