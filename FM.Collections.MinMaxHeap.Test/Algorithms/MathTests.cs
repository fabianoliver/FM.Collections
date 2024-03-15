namespace FM.Collections.Test.Algorithms;

public class LogBase2Tests : MathLogTest<Arity.Two> { }
public class LogBase3Tests : MathLogTest<Arity.Three> { }
public class LogBase4Tests : MathLogTest<Arity.Four> { }
public class LogBase5Tests : MathLogTest<Arity.Five> { }
public class LogBase6Tests : MathLogTest<Arity.Six> { }
public class LogBase7Tests : MathLogTest<Arity.Seven> { }
public class LogBase8Tests : MathLogTest<Arity.Eight> { }
public class LogBase9Tests : MathLogTest<Arity.Nine> { }
public class LogBase10Tests : MathLogTest<Arity.Ten> { }
public class LogBase11Tests : MathLogTest<Arity.Eleven> { }
public class LogBase12Tests : MathLogTest<Arity.Twelve> { }
public class LogBase13Tests : MathLogTest<Arity.Thirteen> { }
public class LogBase14Tests : MathLogTest<Arity.Fourteen> { }
public class LogBase15Tests : MathLogTest<Arity.Fifteen> { }
public class LogBase16Tests : MathLogTest<Arity.Sixteen> { }

public class MathLogTest<TArity> where TArity : struct, IConstInt
{
    [Test]
    public void can_calculate_log_correctly([Range(0U, 1000U, 1U)] uint value)
    {
        var expectedValue = NaiveLog(value, (uint)new TArity().Value);
        Assert.That(FM.Collections.Algorithms.Math.FloorIntLog<TArity>(value), Is.EqualTo(expectedValue));
    }
    
    [Test]
    public void can_calculate_log_correctly_for_uint_maxvalue()
    {
        can_calculate_log_correctly(uint.MaxValue);
    }
    
    [Test]
    public void can_calculate_log_correctly_for_int_maxvalue()
    {
        can_calculate_log_correctly(int.MaxValue);
    }
    
    private static int NaiveLog(uint value, uint @base)
    {
        if (value <= 1)
            return 0;
        
        var result = 0;
        while (value > 0)
        {
            ++result;
            value /= @base;
        }

        return result-1;
    }
}
