using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TokiInterpreter : MonoBehaviour
{
    [SerializeField] private Toki[] searches;

    void Start()
    {
        Box.onSubmit = OnSubmit;
    }

    private Interpreter current;

    public void SetInterpreter(string name)
    {
        current = null;
        foreach (Toki toki in searches)
        {
            if (toki.name == name)
            {
                current = new Interpreter(toki)
                {
                    onMessage = OnMessage,
                    onDestination = OnDestination
                };
                break;
            }
        }

        if (current == null)
        {
            Debug.LogWarning("Couldn't find .toki " + name);
            return;
        }
        Debug.Log("Starting .toki " + name);
        //current.Run();
    }

    private void Update()
    {
        current?.Run();
    }

    private void OnMessage(string message)
    {
        TextManager.CreateMessage(message);
    }

    private void OnDestination(string destination) {
        SetInterpreter(destination);
    }

    private void OnSubmit(Nimi nimi)
    {
        if (current == null)
        {
            Debug.LogWarning("Item submitted but no .toki is running");
            return;
        }
        
        current.SetValueFromWile(nimi);
        current.CallFunction("#item", true);
    }
}

public class Interpreter
{
    private static readonly Dictionary<string, float> globalValues = new();

    private readonly Stack<Expression> stack;
    private readonly Dictionary<string, List<Expression>> functions;
    private readonly Dictionary<string, float> values;
    private readonly List<WileExpression> wile;

    public UnityAction<string> onMessage;
    public UnityAction<string> onDestination;

    private float resumeAt = -1;

    public Interpreter(Toki toki)
    {
        stack = new Stack<Expression>(toki.expressions.Length);
        functions = new();
        values = new();
        wile = new();

        for (int i = toki.expressions.Length - 1; i >= 0; i--)
        {
            stack.Push(toki.expressions[i].expression);
        }
    }

    public void Run()
    {
        if (resumeAt < Time.time)
        {
            resumeAt = -1;
        }

        while (stack.Count > 0 && resumeAt < 0)
        {
            stack.Pop().Run(this);
        }
    }

    public void SetValue(string group, float value)
    {
        GetValueDictionary(group)[group] = value;
    }

    public float GetValue(string group)
    {
        Dictionary<string, float> dictionary = GetValueDictionary(group);
        dictionary.TryGetValue(group, out float f);
        return f;
    }

    private Dictionary<string, float> GetValueDictionary(string group)
    {
        return group[0] == '#' ? globalValues : values;
    }

    public void AddWile(WileExpression wileExpression)
    {
        wile.Add(wileExpression);
    }

    public void SetValueFromWile(Nimi nimi)
    {
        foreach (WileExpression wileExpression in wile)
        {
            (string group, float value) = wileExpression.GetScore(nimi);
            SetValue(group, GetValue(group) + value);
        }
    }

    public void DefineFunction(string name, List<Expression.Wrapper> wrappers)
    {
        List<Expression> expressions = new(wrappers.Count);
        foreach (Expression.Wrapper expression in wrappers)
        {
            //store functions in reverse so they can be added to the stack easily
            expressions.Insert(0, expression.expression);
        }
        functions[name] = expressions;
    }

    public void CallFunction(string name, bool clearStack)
    {
        if (clearStack)
        {
            stack.Clear();
        }

        List<Expression> function = functions[name];
        if (function == null)
        {
            return;
        }

        foreach (Expression expression in function)
        {
            stack.Push(expression);
        }
        resumeAt = -1;
    }

    public void CallExpression(Expression expression)
    {
        stack.Push(expression);
    }

    public void DisplayMessage(string message)
    {
        onMessage.Invoke(message);
    }

    public void Next(string destination)
    {
        stack.Clear();
        onDestination.Invoke(destination);
    }

    public void Suspend(float duration)
    {
        resumeAt = Time.time + duration;
    }
}