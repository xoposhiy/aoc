public class LineReader
{
    public readonly string Line;
    private readonly int lineIndex;
    private int colIndex;

    public LineReader(string line, int lineIndex, int colIndex = 0)
    {
        this.Line = line;
        this.lineIndex = lineIndex;
        this.colIndex = colIndex;
    }

    public bool IsEndOfLine => colIndex >= Line.Length;
    public char CurrentChar => Line[colIndex];

    public override string ToString()
    {
        return $"Line {lineIndex} col {colIndex}: '...{Line.SubstringSafe(colIndex-10, 20)}...'";
    }

    public string ReadLineUntil(string separators)
    {
        SkipSeparators(separators);
        if (IsEndOfLine)
            throw new FormatException($"Unexpected EOL. Line:{lineIndex} col:{colIndex}. String ending: '{Line.SubstringSafe(colIndex-10)}'");
        var start = colIndex;
        while (colIndex < Line.Length && !separators.Contains(Line[colIndex]))
            colIndex++;
        var result = Line.Substring(start, colIndex - start);
        SkipSeparators(separators);
        return result;
        
    }

    public void SkipSeparators(string separators)
    {
        while (colIndex < Line.Length && separators.Contains(Line[colIndex]))
            colIndex++;
    }

    public void MoveToNextChar()
    {
        colIndex++;        
    }
}