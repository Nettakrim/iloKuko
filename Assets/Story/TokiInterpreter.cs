using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter
{
    private readonly Toki toki;
    private readonly List<Expression> queue;
    private readonly Dictionary<string, List<Expression>> functions;
    private readonly Dictionary<string, float> values;
    private readonly List<WileExpression> wile;

    private float resumeAt = -1;

    public Interpreter(Toki toki)
    {
        this.toki = toki;
        queue = new List<Expression>(toki.expressions.Length);
        functions = new();
        wile = new();

        foreach (Expression.Wrapper expression in toki.expressions)
        {
            queue.Add(expression.expression);
        }
    }

    public void Update() {
        if (resumeAt > Time.time)
        {
            resumeAt = -1;
        }

        while (queue.Count > 0 && resumeAt < 0)
        {
            Expression current = queue[0];
            queue.RemoveAt(0);
            current.Step(this);
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

    public void CallFunction(string name)
    {
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
        Debug.Log(toki + " says: " + message);
    }

    public void Next(string destination)
    {
        Debug.Log(toki + " finished, with a target of " + destination);
        queue.Clear();
    }

    public void Suspend(float duration)
    {
        resumeAt = Time.time + duration;
    }
}