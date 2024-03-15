using System;
using System.Runtime.CompilerServices;

namespace FM.Collections.Algorithms;

public static class Math
{
    private static readonly int[] DeBruijnPositions = new int[]
    {
        0, 9, 1, 10, 13, 21, 2, 29, 11, 14, 16, 18, 22, 25, 3, 30,
        8, 12, 20, 28, 15, 17, 24, 7, 19, 27, 23, 6, 26, 5, 4, 31
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint FloorIntLog<TBase>(uint n)
        where TBase : struct, IConstInt
    {
        if (new TBase().Value < 1)
            throw new ArgumentException("base");

        if (new TBase().Value == 1)
            return n;

        if (new TBase().Value == 2)
            return (uint)Log2(n);

        return LogCache<TBase>.LogFloor(n);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Log2(uint index)
    {
#if NETCOREAPP3_0_OR_GREATER
        return System.Numerics.BitOperations.Log2(index);
#else
        return Log2DeBruijn(index);
#endif
    }

    internal static int Log2DeBruijn(uint value)
    {
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;
        return DeBruijnPositions[value * 0x07C4ACDD >> 27];
    }

    // https://stackoverflow.com/questions/63411054/how-can-you-quickly-compute-the-integer-logarithm-for-any-base
    private static class LogCache<TBase> where TBase : struct, IConstInt
    {
#if NET8_0_OR_GREATER
        private static readonly UIntArray32Elements Guesses;
        private static readonly ULongArray22Elements Powers; // Need at most 22 elements assuming base >= 3
#else
        private static readonly uint[] Guesses;
        private static readonly ulong[] Powers;
#endif
        private static readonly uint MaxExp;
        private static readonly uint MaxGuess;

        static LogCache()
        {
            MaxExp = IterativeLog(~0U);
            Guesses = CreateGuessTable();
            Powers = CreatePowTable();
            MaxGuess = Guesses[31];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LogFloor(uint val)
        {
            if (IsPowerOf2Or0((uint)new TBase().Value))
                return (uint)Log2(val) / (uint)Log2((uint)new TBase().Value);

            var guess = Guesses[Log2(val)];

            if (MaxGuess < MaxExp)
                return guess + (val >= Powers[(int)(guess + 1)] ? 1U : 0U);

            return guess + (val / (uint)new TBase().Value >= Powers[(int)guess] ? 1U : 0U);
        }

        private static uint IterativeLog(uint value)
        {
            var result = 0U;
            while ((value /= (uint)new TBase().Value) > 0)
                ++result;
            return result;
        }

#if NET8_0_OR_GREATER
        private static UIntArray32Elements CreateGuessTable()
        {
            var result = new UIntArray32Elements();
            for (var i = 0; i < 32; i++)
                result[i] = IterativeLog(1U << i);
            return result;
        }
        
        private static ULongArray22Elements CreatePowTable()
        {
            var result = new ULongArray22Elements();
            var x = 1UL;
            for (var i = 0; i < MaxExp + 2; ++i, x *= (uint)new TBase().Value)
                result[i] = x;
            return result;
        }
#else
        private static uint[] CreateGuessTable()
         {
            var result = new uint[32];
            for (var i = 0; i < result.Length; i++)
                result[i] = IterativeLog(1U << i);
            return result;
        }
        
private static ulong[] CreatePowTable()
        {
            var result = new ulong[MaxExp + 2];
            var x = 1UL;
            for (var i = 0; i < result.Length; ++i, x *= (uint)new TBase().Value)
                result[i] = x;
            return result;
        }
#endif

       

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPowerOf2Or0(ulong val)
        {
            return (val & (val - 1)) == 0;
        }
    }

#if NET8_0_OR_GREATER
    [InlineArray(32)]
    private struct UIntArray32Elements
    {
        private uint _element0;
    }
    
    [InlineArray(220)]
    private struct ULongArray22Elements
    {
        private ulong _element0;
    }
#endif
}