using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using static System.Math;

public class Day21
{
    public void Solve()
    {
        var recipes = File.ReadAllLines("21.txt")
            .Select(line => line.Split(new[] { " (contains ", ")" }, StringSplitOptions.RemoveEmptyEntries))
            .Select(line => (ingridients: line[0].Split(" ").ToList(), allergens: line[1].Split(", ")))
            .ToList();

        var options = recipes
            .SelectMany(r => r.allergens.Select(allergen => (allergen, ingredients: r.ingridients)))
            .GroupBy(pair => pair.allergen)
            .Select(allergenGroup => (
                allergen: allergenGroup.Key,
                ingredients: allergenGroup.Select(t => t.ingredients).Aggregate((acc, next) => acc.Intersect(next).ToList())
                ))
            .ToList();
        
        var used = new HashSet<string>();
        for (int i = 0; i < options.Count; i++)
        {
            var option = options.First(o => o.ingredients.Count == 1 && used.Add(o.allergen));
            var ing = option.ingredients.Single();
            foreach (var other in options.Where(other => other != option))
                other.ingredients.Remove(ing);
        }

        Console.WriteLine(options.StrJoin("\n", pair => $"{pair.allergen}: {pair.ingredients.StrJoin(" ")}"));
        Console.WriteLine();

        var alergensByIng = options.ToDictionary(o => o.ingredients.Single(), o => o.allergen);
        var ans1 = recipes.Sum(r => r.ingridients.Count(ing => !alergensByIng.ContainsKey(ing)));

        Console.WriteLine($"Part One: {ans1}");

        var ans2 = alergensByIng.OrderBy(kv => kv.Value).Select(kv => kv.Key).StrJoin(",");
        Console.WriteLine($"Part Two: {ans2}");
    }
}