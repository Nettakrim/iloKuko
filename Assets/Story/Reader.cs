using System.IO;

public class Reader
{
    private readonly StreamReader reader;
    private string current;

    public Reader(StreamReader reader)
    {
        this.reader = reader;
        current = null;
    }

    public bool Next()
    {
        while (!reader.EndOfStream)
        {
            current = reader.ReadLine();
            if (!string.IsNullOrWhiteSpace(current) && !(current[0] == '#'))
            {
                current = current.TrimStart();
                return true;
            }
        }
        current = null;
        return false;
    }

    public void SetLine(string to)
    {
        current = to;
    }

    public string GetLine()
    {
        return current;
    }
}