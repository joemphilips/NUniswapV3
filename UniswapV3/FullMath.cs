using System.Numerics;
using CustomInt256;

namespace UniswapV3;

public static class FullMath
{
    private static BigInteger? _UInt512Max {
        get;
        set;
    }

    private static BigInteger UInt512Max {
        get
        {
            if (_UInt512Max != null)
            {
                return _UInt512Max.Value;
            }

            UInt256.MaxValue.Convert(out var t);
            _UInt512Max = t * 2;
            return _UInt512Max.Value;
        }
    }
    
    public static UInt256 MulDiv(UInt256 a, UInt256 b, UInt256 denominator)
    {
        var a2 = (BigInteger)a;
        var b2 = (BigInteger)b;
        var denom2 = (BigInteger)denominator;

        var result = a2 * b2 / denom2;
        if (!(result <= (BigInteger)UInt256.MaxValue))
        {
            throw new OverflowException($"Result must be less than uint256 max {result.ToString()}");
        }

        return new UInt256(
            result.ToBytes32(true),
            true
        );
    }

    public static UInt256 MulDivRoundingUp(UInt256 a, UInt256 b, UInt256 denominator)
    {
        var result = MulDiv(a, b, denominator);
        a.MultiplyMod(b, denominator, out var mod);
        if (mod > 0)
        {
            if (!(result < UInt256.MaxValue))
            {
                throw new InvalidDataException("Foo");
            }
            result++;
        }

        return result;
    }
}