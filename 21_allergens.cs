using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Linq.Enumerable;
using static System.Math;

public class Day21
{
    
    
    
    public void Solve()
    {
        var recipes = File.ReadAllLines("21.txt")
            .Select(line => line.Split(new[] { " (contains ", ")" }, StringSplitOptions.RemoveEmptyEntries))
            .Select(line => (ingridients: line[0].Split(" "), allergens: line[1].Split(", ")))
            .ToList();

        var ingByAllergen = recipes
            .SelectMany(r => r.allergens.Select(allergen => (allergen, ingredients: r.ingridients)))
            .GroupBy(pair => pair.allergen, pair => pair.ingredients)
            .Select(allergenGroup => (
                allergen: allergenGroup.Key,
                ingredients: allergenGroup.IntersectAll(t => t)
            )).FindBijection();
        
        Console.WriteLine(ingByAllergen.StrJoin("\n", pair => $"{pair.Key}: {pair.Value}"));
        Console.WriteLine();

        var allergensByIng = ingByAllergen.ToDictionary(o => o.Value, o => o.Key);
        var ans1 = recipes.Sum(r => r.ingridients.Count(ing => !allergensByIng.ContainsKey(ing)));

        Console.WriteLine($"Part One: {ans1}");

        var ans2 = allergensByIng.OrderBy(kv => kv.Value).Select(kv => kv.Key).StrJoin(",");
        Console.WriteLine($"Part Two: {ans2}");
    }
}