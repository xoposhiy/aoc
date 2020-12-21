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
            .Select(line => (ingridients: line[0].Split(" "), allergens: line[1].Split(", ")))
            .ToList();

        var allergenOptions = recipes
            .SelectMany(r => r.allergens.Select(allergen => (allergen, ingredients: r.ingridients)))
            .GroupBy(pair => pair.allergen)
            .Select(allergenGroup => (
                allergen: allergenGroup.Key,
                ingredients: allergenGroup.IntersectAll(t => t.ingredients)
                ))
            .ToList();
        
        var used = new HashSet<string>();
        foreach (var _ in allergenOptions)
        {
            var option = allergenOptions
                .First(o => o.ingredients.Count == 1 && used.Add(o.allergen));
            foreach (var (_, ingredients) in allergenOptions.Where(other => other != option))
                ingredients.Remove(option.ingredients.Single());
        }

        Console.WriteLine(allergenOptions.StrJoin("\n", pair => $"{pair.allergen}: {pair.ingredients.StrJoin(" ")}"));
        Console.WriteLine();

        var allergensByIng = allergenOptions.ToDictionary(o => o.ingredients.Single(), o => o.allergen);
        var ans1 = recipes.Sum(r => r.ingridients.Count(ing => !allergensByIng.ContainsKey(ing)));

        Console.WriteLine($"Part One: {ans1}");

        var ans2 = allergensByIng.OrderBy(kv => kv.Value).Select(kv => kv.Key).StrJoin(",");
        Console.WriteLine($"Part Two: {ans2}");
    }
}