using System.Text.Json.Nodes;
using Shouldly;

namespace ParsingTests;

public class Tests
{
    [TestCase("abc \n 42")]
    public void NoParsing(string input)
    {
        void Solve(string text) => text.ShouldBe("abc \n 42");
        Run(Solve, input.Split('\n'));
    }

    [TestCase("42")]
    public void JustOneInt(string input)
    {
        void Solve(int a) => a.ShouldBe(42);
        Run(Solve, input.Split('\n'));
    }

    [TestCase("abc \n 42")]
    public void JustSplitToLines(string input)
    {
        void Solve(string[] lines) => lines.Length.ShouldBe(2);
        Run(Solve, input.Split('\n'));
    }

    [TestCase("1\n2\n3\n4")]
    [TestCase(" 1\n   2\n3 \n 4")]
    public void ArrayOfNumbers(string input)
    {
        void Solve(int[] numbers) => numbers.Length.ShouldBe(4);
        Run(Solve, input.Split('\n'));
    }

    [TestCase("1\n2 3 4\n\n4")]
    [TestCase("1\n2 3 4\n1 2\n4")]
    public void ArrayOfArrays(string input)
    {
        void Solve(int[][] lines) => lines.Length.ShouldBe(4);
        Run(Solve, input.Split('\n'));
    }

    [TestCase("""
    1 a
       2 b
     3 x
    """)]
    
    public void ArrayOfTuples(string input)
    {
        void Solve((int a, char c)[] lines) => lines.Sum(l => l.a).ShouldBe(6);
        Run(Solve, input.Split('\n'));
    }
    
    [TestCase("""
    1→a
    2→ → → b
    3→→x
    """)]
    public void ManualSeparators(string input)
    {
        [Separators("→")]
        void Solve((int a, char c)[] lines) => lines.Sum(l => l.a).ShouldBe(6);
        Run(Solve, input.Split('\n'));
    }
    
    public enum Color { Red, Green, Blue }

    [TestCase("""
    Red
    Green
    Blue
    """)]
    public void Enums(string input)
    {
        void Solve(Color[] lines) => lines.Distinct().Count().ShouldBe(3);
        Run(Solve, input.Split('\n'));
    }
    
    [TestCase("""
    { "a": 42, "b": 2 }
    """)]
    public void JsonObject(string input)
    {
        void Solve(JsonObject obj) => obj["a"]?.GetValue<int>().ShouldBe(42);
        Run(Solve, input.Split('\n'));
    }

    [TestCase("[\n]")]
    [TestCase("{\n}")]
    public void JsonNode(string input)
    {
        void Solve(JsonNode obj) => obj.ShouldNotBeNull();
        Run(Solve, input.Split('\n'));
    }
    
    [TestCase("Pavel\n42")]
    public void Object(string input)
    {
        void Solve((string name, int age) person) => person.age.ShouldBe(42);
        Run(Solve, input.Split('\n'));
    }
    
    [Template("Name: @Name;\nAge: @Age")]
    public record Person(string Name, int Age);
    
    [TestCase("Name: Pavel;\nAge: 42")]
    public void ObjectWithTemplate(string input)
    {
        void Solve(Person person) => person.Age.ShouldBe(42);
        Run(Solve, input.Split('\n'));
    }

    [TestCase("123\n456")]
    public void CharMap(string input)
    {
        void Solve(char[][] map) => map[1][2].ShouldBe('6');
        Run(Solve, input.Split('\n'));
    }

    [TestCase("123\n456")]
    public void IntMap(string input)
    {
        void Solve(int[][] map) => map[1][2].ShouldBe(6);
        Run(Solve, input.Split('\n'));
    }
    
    //TODO:
    // + single block
    // array of blocks
    // list of blocks
    // Own separators for block

    // + char[][] map
    // + int[][] map
    // + block as multiline string
    // + block as multiline json
    // + block as array (line = element)
    // + block as object (line = argument) with template
    // + block as object (line = argument) without template

    // + block item is string 
    // block item is object by template 
    // block item is object without template
    // block item is JsonNode | JsonObject | JsonValue | JsonArray
    // block item is array of values
    // block item is array of objects
    // + block item is int | long | double | string | char | enum

    private void Run(Delegate solve, string[] input)
    {
        solve.Method.InvokeWithParsedArgs(null!, input);
    }
}