namespace FM.Collections.Test;

public sealed class CaseInsensitiveString(string value)
{
    public string Value { get; } = value;

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }
    
    public override bool Equals(object? obj)
    {
        return StringComparer.OrdinalIgnoreCase.Equals(Value, ((CaseInsensitiveString?)obj)?.Value);
    }
}