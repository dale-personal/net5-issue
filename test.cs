using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

public struct Text : IEquatable<Text>
{
    private readonly string _value;

    public Text(char[] value)
        => _value = new string(value);

    public Text(string value)
        => _value = value;

    public string Value
        => _value ?? string.Empty; 

    public override string ToString()
        => _value ?? string.Empty;

    public override bool Equals(object obj)
    {
        if (obj is Text txt) return txt.Value == Value;
        if (obj is string str) return str == Value;
        return false;
    }

    public override int GetHashCode()
        => _value == null ? 0 : _value.GetHashCode();

    public bool Equals(Text other)
        => other.Value == Value;

    public static implicit operator Text(string value) 
        => new Text(value);

    public static implicit operator Text(char[] value) 
        => new Text(value);

}

public class Rules<T> where T : struct
{
    public Rules(T value)
        => Value = value;

    public T? Value { get; }
}

public abstract class Property<TValue> where TValue : struct
{
    private TValue? _value;
    private Rules<TValue> _required;
    private Rules<TValue> _default;

    public TValue GetValue(TValue? initial = null)
    {
        if (_required != null && _required.Value.HasValue) return _required.Value.Value;
        if (_value.HasValue) return _value.Value;
        if (initial.HasValue) return initial.Value;

        // Strange things eventually here.
        if (_default != null && _default.Value.HasValue) return _default.Value.Value;
        return default;

        //return _defaultValues?.Value ?? default;
    }

    public void SetRequired(TValue value) 
        => _required = new Rules<TValue>(value);

    public void SetDefault(TValue value) 
        => _default = new Rules<TValue>(value);
    
    public void Set(TValue value) 
        => _value = value;

}

public class TextProperty : Property<Text> { }

/// <summary>
/// Running 'dotnet test -c release' fails eventually at around 300k iterations.
/// </summary>
[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        var test = "test";
        var l = new System.Collections.Generic.List<TextProperty>();
        for (int i = 0; i < 1000000; i++)
        {
            var t = new TextProperty();
            l.Add(t);
            t.SetDefault(test);
            Assert.AreEqual(test, t.GetValue(), $"interation: {i}");
        }
    }
}
