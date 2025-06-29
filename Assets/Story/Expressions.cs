using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public abstract class Expression
{
    public abstract void Run(Interpreter interpreter);

    #if UNITY_EDITOR
    protected abstract void ParseExpression(Reader reader, string args);

    public static Expression ParseReader(Reader reader)
    {
        string line = reader.GetLine();
        string[] parts = line.Split('/', 2, System.StringSplitOptions.None);

        Expression expression = parts[0] switch
        {
            "def"  => new DefExpression(),
            "call" => new CallExpression(),
            "set"  => new SetExpression(),
            "test" => new TestExpression(),
            "toki" => new TokiExpression(),
            "wile" => new WileExpression(),
            "wait" => new WaitExpression(),
            "next" => new NextExpression(),
            "reject" => new RejectExpression(),
            "stage" => new StageExpression(),
            _ => null
        };

        if (expression == null)
        {
            Debug.LogWarning("Couldnt parse line " + reader.GetLine());
            return null;
        }

        expression?.ParseExpression(reader, parts[1].TrimStart());
        return expression;
    }
    #endif

    [System.Serializable]
    public class Wrapper
    {
        [SerializeReference] public Expression expression;

        public Wrapper(Expression expression)
        {
            this.expression = expression;
        }

        public static implicit operator Wrapper(Expression expression)
        {
            return new Wrapper(expression);
        }
    }
}

public abstract class MultiLineExpression : Expression
{
    #if UNITY_EDITOR
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
    #endif
}

[System.Serializable]
public class DefExpression : MultiLineExpression
{
    [SerializeField] private string name;
    [SerializeField] private List<Wrapper> expressions = new();

    public override void Run(Interpreter interpreter)
    {
        interpreter.DefineFunction(name, expressions);
    }

    #if UNITY_EDITOR
    protected override void ParseArgs(string args)
    {
        name = args;
    }

    protected override void ParseLine(Reader reader)
    {
        expressions.Add(ParseReader(reader));
    }
    #endif
}

[System.Serializable]
public class CallExpression : Expression
{
    [SerializeField] private string function;

    public override void Run(Interpreter interpreter)
    {
        interpreter.CallFunction(function);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        function = args;
    }
    #endif
}

[System.Serializable]
public class SetExpression : Expression
{
    [SerializeField] private float value;
    [SerializeField] private string group;

    public override void Run(Interpreter interpreter)
    {
        interpreter.SetValue(group, value);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        int i = args.IndexOf(' ');
        value = float.Parse(args[..i], CultureInfo.InvariantCulture);
        group = args[i..].TrimStart();
    }
    #endif
}

[System.Serializable]
public class TestExpression : MultiLineExpression
{
    [SerializeField] private List<TestData> testDatas = new();

    public override void Run(Interpreter interpreter)
    {
        foreach (TestData testData in testDatas)
        {
            if (testData.Passes(interpreter))
            {
                testData.Run(interpreter);
                return;
            }
        }
    }

    #if UNITY_EDITOR
    protected override void ParseArgs(string args)
    {

    }

    protected override void ParseLine(Reader reader)
    {
        string s = reader.GetLine();
        int i = s.IndexOf(' ');
        float amount = float.Parse(s[..i], CultureInfo.InvariantCulture);

        s = s[i..].TrimStart();
        i = s.IndexOf(' ');
        string group = s[..i];

        reader.SetLine(s[i..].TrimStart());
        Expression expression = ParseReader(reader);

        testDatas.Add(new TestData(amount, group, expression));
    }
    #endif

    [System.Serializable]
    public class TestData
    {
        #if UNITY_EDITOR
        public TestData(float amount, string group, Expression expression)
        {
            this.amount = amount;
            this.group = group;
            this.expression = expression;
        }
        #endif

        [SerializeField] private float amount;
        [SerializeField] private string group;
        [SerializeReference] private Expression expression;

        public bool Passes(Interpreter interpreter)
        {
            return interpreter.GetValue(group) + 0.01f >= amount;
        }

        public void Run(Interpreter interpreter)
        {
            interpreter.CallExpression(expression);
        }
    }
}

[System.Serializable]
public class TokiExpression : Expression
{
    [SerializeField] private string message;

    public override void Run(Interpreter interpreter)
    {
        interpreter.DisplayMessage(message);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        message = args;
    }
    #endif
}

[System.Serializable]
public class WileExpression : Expression
{
    [SerializeField] private float multiplier;
    [SerializeField] private string group;
    [SerializeField] private string[] nimi;

    public override void Run(Interpreter interpreter)
    {
        interpreter.AddWile(this);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        int i = args.IndexOf(' ');
        multiplier = float.Parse(args[..i], CultureInfo.InvariantCulture);

        args = args[i..].TrimStart();
        i = args.IndexOf(' ');
        group = args[..i];

        nimi = args[i..].TrimStart().Split(new char[] { ' ', ',' });
    }
    #endif

    public (string, float) GetScore(Nimi nimi)
    {
        float total = 0;

        for (int i = 0; i < nimi.words.Length; i++)
        {
            foreach (string word in nimi.words[i])
            {
                if (this.nimi.Contains(word))
                {
                    total += multiplier / (i + 1);
                }
            }
        }

        return (group, total);
    }
}

[System.Serializable]
public class WaitExpression : Expression
{
    [SerializeField] private float duration;

    public override void Run(Interpreter interpreter)
    {
        interpreter.Suspend(duration);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        duration = float.Parse(args, CultureInfo.InvariantCulture);
    }
    #endif
}

[System.Serializable]
public class NextExpression : Expression
{
    [SerializeField] private string destination;

    public override void Run(Interpreter interpreter)
    {
        interpreter.Next(destination);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        destination = args;
    }
    #endif
}

[System.Serializable]
public class RejectExpression : Expression
{
    public override void Run(Interpreter interpreter)
    {
        interpreter.Reject();
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {

    }
    #endif
}

[System.Serializable]
public class StageExpression : Expression
{
    [SerializeField] private string direction;

    public override void Run(Interpreter interpreter)
    {
        interpreter.Stage(direction);
    }

    #if UNITY_EDITOR
    protected override void ParseExpression(Reader reader, string args)
    {
        direction = args;
    }
    #endif
}
