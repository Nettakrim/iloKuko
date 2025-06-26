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
        SetInterpreter(searches[0].name);
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
            Debug.LogError("Couldn't find .toki " + name);
            return;
        }
        Debug.Log("Starting .toki " + name);
    }

    private void Update()
    {
        current?.Run();
    }

    private void OnMessage(string message) {
        Debug.Log("says: " + message);
    }

    private void OnDestination(string destination) {
        SetInterpreter(destination);
    }
}

public class Interpreter
{
    private readonly Toki toki;
    private readonly List<Expression> queue;
    private readonly Dictionary<string, List<Expression>> functions;
    private readonly Dictionary<string, float> values;
    private readonly List<WileExpression> wile;

    public UnityAction<string> onMessage;
    public UnityAction<string> onDestination;

    private float resumeAt = -1;

    public Interpreter(Toki toki)
    {
        this.toki = toki;
        queue = new List<Expression>(toki.expressions.Length);
        functions = new();
        values = new();
        wile = new();

        foreach (Expression.Wrapper expression in toki.expressions)
        {
            queue.Add(expression.expression);
        }
    }

    public void Run()
    {
        if (resumeAt < Time.time)
        {
            resumeAt = -1;
        }

        while (queue.Count > 0 && resumeAt < 0)
        {
            Expression current = queue[0];
            queue.RemoveAt(0);
            current.Run(this);
        }
    }

    public void SetValue(string group, float value)
    {
        values[group] = value;
    }

    public float GetValue(string group)
    {
        return values[group];
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
            values[group] += value;
        }
    }

    public void DefineFunction(string name, List<Expression.Wrapper> wrappers)
    {
        List<Expression> expressions = new(wrappers.Count);
        foreach (Expression.Wrapper expression in toki.expressions)
        {
            expressions.Add(expression.expression);
        }
        functions.Add(name, expressions);
    }

    public void CallFunction(string name, bool clearQueue)
    {
        if (clearQueue)
        {
            queue.Clear();
        }

        List<Expression> function = functions[name];
        if (function == null)
        {
            return;
        }

        queue.InsertRange(0, function);
        resumeAt = -1;
    }

    public void CallExpression(Expression expression)
    {
        queue.Insert(0, expression);
    }

    public void DisplayMessage(string message)
    {
        onMessage.Invoke(message);
    }

    public void Next(string destination)
    {
        queue.Clear();
        onDestination.Invoke(destination);
    }

    public void Suspend(float duration)
    {
        resumeAt = Time.time + duration;
    }
}