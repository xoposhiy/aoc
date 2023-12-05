using System.Text;

public class MultilineReader
{
    private readonly string[] lines;
    private int ln;
    public string CurrentLine => lines[ln];
    public bool IsEndOfLines => ln >= lines.Length;

    public LineReader GetLineReader() => new(lines[ln], ln);

    public void MoveToNextLine()
    {
        ln++;
    }
    
    public MultilineReader(string[] lines)
    {
        this.lines = lines;
        ln = 0;
    }

    public override string ToString()
    {
        return $"Line {ln}: '{lines[ln]}'";
    }

    public string ReadLine()
    {
        return lines[ln++];
    }

    public string ReadUntilEmptyLine()
    {
        var sb = new StringBuilder();
        while (ln < lines.Length && !string.IsNullOrEmpty(lines[ln]))
            sb.AppendLine(lines[ln++]);
        if (ln < lines.Length)
            ln++; // skip empty line;
        return sb.ToString();
    }
}