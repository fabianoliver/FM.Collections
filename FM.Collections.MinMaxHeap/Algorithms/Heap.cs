using System.Runtime.CompilerServices;

namespace FM.Collections.Algorithms;

public static class Heap
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetParentIndex(int arity, int index) => (index - 1) / arity;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetGrandParentIndex(int arity, int index) => (index - (arity + 1)) / (arity * arity);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetFirstChildIndex(int arity, int index) => index * arity + 1;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLastChildIndex(int arity, int index) => index * arity + arity;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLastGrandChildIndex(int arity, int index) => index * arity * arity + arity * arity + arity;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetFirstGrandChildIndex(int arity, int index) => index * arity * arity + arity + 1;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetLevel<TArity>(int index) where TArity : struct, IConstInt => (int)Math.FloorIntLog<TArity>((uint)(index * (new TArity().Value - 1) + 1));
}