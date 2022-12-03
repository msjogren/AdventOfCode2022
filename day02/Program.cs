const char OpponentMove_Rock = 'A',
           OpponentMove_Paper = 'B',
           OpponentMove_Scissors = 'C';
const char MyMove_Rock = 'X',
           MyMove_Paper = 'Y',
           MyMove_Scissors = 'Z';
const char Outcome_Lose = 'X',
           Outcome_Draw = 'Y',
           Outcome_Win = 'Z';
const int Score_Lose = 0,
          Score_Draw = 3,
          Score_Win = 6;
const int Score_Rock = 1,
          Score_Paper = 2,
          Score_Scissors = 3;

var input = File.ReadAllLines("input.txt").Select(line => (line[0], line[2]));

// Part 1
var totalScore = input.Sum(round => round switch {
        (OpponentMove_Rock,     MyMove_Rock)     => Score_Draw + Score_Rock,
        (OpponentMove_Rock,     MyMove_Paper)    => Score_Win  + Score_Paper,
        (OpponentMove_Rock,     MyMove_Scissors) => Score_Lose + Score_Scissors,
        (OpponentMove_Paper,    MyMove_Rock)     => Score_Lose + Score_Rock,
        (OpponentMove_Paper,    MyMove_Paper)    => Score_Draw + Score_Paper,
        (OpponentMove_Paper,    MyMove_Scissors) => Score_Win  + Score_Scissors,
        (OpponentMove_Scissors, MyMove_Rock)     => Score_Win  + Score_Rock,
        (OpponentMove_Scissors, MyMove_Paper)    => Score_Lose + Score_Paper,
        (OpponentMove_Scissors, MyMove_Scissors) => Score_Draw + Score_Scissors,
        _ => throw new ArgumentOutOfRangeException()
    });
Console.WriteLine(totalScore);

// Part 2
var totalScore2 = input.Sum(round => round switch {
        (OpponentMove_Rock,     Outcome_Lose) => Score_Lose + Score_Scissors,
        (OpponentMove_Rock,     Outcome_Draw) => Score_Draw + Score_Rock,
        (OpponentMove_Rock,     Outcome_Win)  => Score_Win  + Score_Paper,
        (OpponentMove_Paper,    Outcome_Lose) => Score_Lose + Score_Rock,
        (OpponentMove_Paper,    Outcome_Draw) => Score_Draw + Score_Paper,
        (OpponentMove_Paper,    Outcome_Win)  => Score_Win  + Score_Scissors,
        (OpponentMove_Scissors, Outcome_Lose) => Score_Lose + Score_Paper,
        (OpponentMove_Scissors, Outcome_Draw) => Score_Draw + Score_Scissors,
        (OpponentMove_Scissors, Outcome_Win)  => Score_Win  + Score_Rock,
        _ => throw new ArgumentOutOfRangeException()
    });
Console.WriteLine(totalScore2);
