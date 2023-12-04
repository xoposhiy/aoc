// https://adventofcode.com/2023/day/2
/*
    Input lines with format: [Game 1: 12 red, 2 green, 5 blue; 9 red, 6 green, 4 blue; 10 red, 2 green, 5 blue; 8 blue, 9 red]
    Part 1:
    - Sum of all game ids where all turns have less than 12 red, 13 green and 14 blue
    Part 2:
    - Sum of all games its power — the product of max count of each color
 */

using System.Xml.Serialization;

public class Day02
{
    public enum Color { Red, Green, Blue }

    public record Game(string _, int Id, (int Count, Color Color)[] Turns);
    
    public void Solve(Game[] games)
    {
        bool CanPlay(Game game)
        {
            var actual = game.Turns
                .GroupBy(g => g.Color)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Max(turn => turn.Count));
            return actual[Color.Red] <= 12 && actual[Color.Green] <= 13 && actual[Color.Blue] <= 14;
        }

        games.Where(CanPlay).Sum(g => g.Id)
            .Part1();
        
        games.Sum(game => 
            game.Turns
                .GroupBy(turn => turn.Color)
                .Select(g => g.Max(turn => turn.Count))
                .Product()
            )
        .Part2();
    }
}