using CustomInt256;

namespace UniswapV3;

public static class UnsafeMath
{
    public static UInt256 DivRoundingUp(UInt256 a, UInt256 b)
    {
        var q = a / b;
        UInt256.Mod(a, b, out var mod);
        return mod.IsZero ? q : q + 1;
    }
}