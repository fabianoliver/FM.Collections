namespace FM.Collections.Test.Algorithms;

public class MinMaxHeapTests
{
    [Test]
    public void can_identify_valid_minmax_heap()
    {
        var heap = new double[] { 0, 10, 14, 3, 1, 5, 2, 6, 8, 9, 4, 11, 12, 13 };
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(heap, Comparer<double>.Default, default(Arity.Two)), Is.True);
    }

    [Test]
    public void can_identify_invalid_minmax_heap()
    {
        var heap = new double[] { 0, 10, 14, 3, 1, 5, 2, 13, 8, 9, 4, 11, 12 };
        Assert.That(FM.Collections.Algorithms.MinMaxHeap.IsMinMaxHeap(heap, Comparer<double>.Default, default(Arity.Two)), Is.False);
    }
}