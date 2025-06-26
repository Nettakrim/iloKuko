using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using UnityEngine;

public abstract class Expression
{
    protected abstract void ParseExpression(Reader reader, string args);

    public static Expression ParseReader(Reader reader)
    {
        string line = reader.GetLine();
        string[] parts = line.Split('/', 2, System.StringSplitOptions.None);

        Expression expression = parts[0] switch
        {
            "def" => new DefExpression(),
            "call" => new CallExpression(),
            "test" => new TestExpression(),
            "toki" => new TokiExpression(),
            "wile" => new WileExpression(),
            "wait" => new WaitExpression(),
            "next" => new NextExpression(),
            _ => null
        };

        if (expression == null)
        {
            Debug.LogWarning("Couldnt parse line "+reader.GetLine());
            return null;
        }

        expression?.ParseExpression(reader, parts[1].TrimStart());
        return expression;
    }

    [System.Serializable]
    public class Wrapper {
        [SerializeReference] public Expression expression;

        public Wrapper(Expression expression) {
            this.expression = expression;
        }

        public static implicit operator Wrapper(Expression expression) {
            return new Wrapper(expression);
        }
    }
}

public abstract class MultiLineExpression : Expression
{
    protected override void ParseExpression(Reader reader, string args)
    {
        ParseArgs(args);
        while (reader.Next())
        {
            if (reader.GetLine().Equals("end/"))
            {
                return;
            }
            else
            {
                ParseLine(reader);
            }
        }
    }

    protected abstract void ParseArgs(string args);

    protected abstract void ParseLine(Reader reader);
}

[System.Serializable]
public class DefExpression : MultiLineExpression
{
    [SerializeField] private string name;
    [SerializeField] private List<Wrapper> expressions = new();

    protected override void ParseArgs(string args)
    {
        name = args;
    }

    protected override void ParseLine(Reader reader)
    {
        expressions.Add(ParseReader(reader));
    }
}

[System.Serializable]
public class CallExpression : Expression
{
    [SerializeField] private string function;

    protected override void ParseExpression(Reader reader, string args)
    {
        function = args;
    }
}

[System.Serializable]
public class TestExpression : MultiLineExpression
{
    [SerializeField] private List<TestData> testDatas = new();

    protected override void ParseArgs(string args)
    {

    }

    protected override void ParseLine(Reader reader)
    {
        TestData testData = new TestData();

        string s = reader.GetLine();
        int i = s.IndexOf(' ');
        testData.amount = float.Parse(s[..i], CultureInfo.InvariantCulture);

        s = s[i..].TrimStart();
        i = s.IndexOf(' ');
        testData.group = s[..i];

        reader.SetLine(s[i..].TrimStart());
        testData.expression = ParseReader(reader);

        testDatas.Add(testData);
    }

    [System.Serializable]
    public class TestData
    {
        public float amount;
        public string group;
        [SerializeReference] public Expression expression;
    }
}

[System.Serializable]
public class TokiExpression : Expression
{
    [SerializeField] private string message;

    protected override void ParseExpression(Reader reader, string args)
    {
        message = args;
    }
}

[System.Serializable]
public class WileExpression : Expression
{
    public float multiplier;
    public string group;
    public string[] nimi;

    protected override void ParseExpression(Reader reader, string args)
    {
        int i = args.IndexOf(' ');
        multiplier = float.Parse(args[..i], CultureInfo.InvariantCulture);

        args = args[i..].TrimStart();
        i = args.IndexOf(' ');
        group = args[..i];

        nimi = args[i..].TrimStart().Split(new char[] { ' ', ',' });
    }
}

[System.Serializable]
public class WaitExpression : Expression
{
    public float duration;

    protected override void ParseExpression(Reader reader, string args)
    {
        duration = float.Parse(args, CultureInfo.InvariantCulture);
    }
}

[System.Serializable]
public class NextExpression : Expression
{
    [SerializeField] private string destination;

    protected override void ParseExpression(Reader reader, string args)
    {
        destination = args;
    }
}