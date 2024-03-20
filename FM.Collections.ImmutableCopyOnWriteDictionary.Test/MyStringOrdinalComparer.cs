namespace FM.Collections.Test;

public class MyStringOrdinalComparer : EqualityComparer<CaseInsensitiveString>
{
    public override bool Equals(CaseInsensitiveString x, CaseInsensitiveString y)
    {
        return StringComparer.Ordinal.Equals(x.Value, y.Value);
    }

    public override int GetHashCode(CaseInsensitiveString obj)
    {
        return StringComparer.Ordinal.GetHashCode(obj.Value);
    }
}
