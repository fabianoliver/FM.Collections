using FM.Collections.Algorithms;

namespace FM.Collections.Test.Algorithms;

public class HeapTests
{
    [TestCase(0, ExpectedResult = 1)]
    [TestCase(1, ExpectedResult = 5)]
    [TestCase(2, ExpectedResult = 9)]
    [TestCase(3, ExpectedResult = 13)]
    [TestCase(3, ExpectedResult = 13)]
    [TestCase(5, ExpectedResult = 21)]
    public int can_identify_child(int index)
    {
        return Heap.GetFirstChildIndex(4, index);
    }

    [TestCase(0, ExpectedResult = 5)]
    [TestCase(1, ExpectedResult = 21)]
    public int can_identify_grandchild(int index)
    {
        return Heap.GetFirstGrandChildIndex(4, index);
    }

    [TestCase(1, ExpectedResult = 0)]
    [TestCase(2, ExpectedResult = 0)]
    [TestCase(3, ExpectedResult = 0)]
    [TestCase(4, ExpectedResult = 0)]
    [TestCase(5, ExpectedResult = 1)]
    [TestCase(6, ExpectedResult = 1)]
    [TestCase(7, ExpectedResult = 1)]
    [TestCase(8, ExpectedResult = 1)]
    [TestCase(9, ExpectedResult = 2)]
    public int can_identify_parent(int index)
    {
        return Heap.GetParentIndex(4, index);
    }

    [TestCase(5, ExpectedResult = 0)]
    [TestCase(21, ExpectedResult = 1)]
    public int can_identify_grandparent(int index)
    {
        return Heap.GetGrandParentIndex(4, index);
    }

    [Test]
    public void grandchild_is_same_as_child_of_child([Range(0, 100)] int index)
    {
        var grandchild = Heap.GetFirstGrandChildIndex(4, index);
        var childOfChild = Heap.GetFirstChildIndex(4, Heap.GetFirstChildIndex(4, index));
        Assert.That(grandchild, Is.EqualTo(childOfChild));
    }


    [TestCase(0, ExpectedResult = true)]
    [TestCase(1, ExpectedResult = false)]
    [TestCase(2, ExpectedResult = false)]
    [TestCase(3, ExpectedResult = true)]
    [TestCase(4, ExpectedResult = true)]
    [TestCase(5, ExpectedResult = true)]
    [TestCase(6, ExpectedResult = true)]
    [TestCase(7, ExpectedResult = false)]
    [TestCase(8, ExpectedResult = false)]
    [TestCase(9, ExpectedResult = false)]
    [TestCase(10, ExpectedResult = false)]
    [TestCase(11, ExpectedResult = false)]
    [TestCase(12, ExpectedResult = false)]
    [TestCase(13, ExpectedResult = false)]
    [TestCase(14, ExpectedResult = false)]
    [TestCase(15, ExpectedResult = true)]
    public bool can_identify_min_layers_arity2(int index)
    {
        return can_identify_min_layers<Arity.Two>(index);
    }

    [TestCase(0, ExpectedResult = true)]
    [TestCase(1, ExpectedResult = false)]
    [TestCase(2, ExpectedResult = false)]
    [TestCase(3, ExpectedResult = false)]
    [TestCase(4, ExpectedResult = false)]
    [TestCase(5, ExpectedResult = true)]
    [TestCase(6, ExpectedResult = true)]
    [TestCase(7, ExpectedResult = true)]
    [TestCase(8, ExpectedResult = true)]
    [TestCase(9, ExpectedResult = true)]
    [TestCase(10, ExpectedResult = true)]
    [TestCase(11, ExpectedResult = true)]
    [TestCase(12, ExpectedResult = true)]
    [TestCase(13, ExpectedResult = true)]
    [TestCase(14, ExpectedResult = true)]
    [TestCase(15, ExpectedResult = true)]
    [TestCase(16, ExpectedResult = true)]
    [TestCase(17, ExpectedResult = true)]
    [TestCase(18, ExpectedResult = true)]
    [TestCase(19, ExpectedResult = true)]
    [TestCase(20, ExpectedResult = true)]
    [TestCase(21, ExpectedResult = false)]
    public bool can_identify_min_layers_arity4(int index)
    {
        return can_identify_min_layers<Arity.Four>(index);
    }

    private bool can_identify_min_layers<T>(int index) where T : struct, IConstInt
    {
        return FM.Collections.Algorithms.MinMaxHeap.IsMinLevel<T>(index);
    }
}