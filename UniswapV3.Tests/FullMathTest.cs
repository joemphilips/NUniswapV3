namespace UniswapV3.Tests;

using CustomInt256;
using System;

public class FullMathTest
{
    private static UInt256 Q128 = new UInt256(0, 0, 1, 0);
    [Fact]
    public void TestMulDiv()
    {
        // Revert if denominator is zero
        Assert.Throws<DivideByZeroException>(() => FullMath.MulDiv(Q128, 5, 0));
        
        // Revert if denominator is zero and numerator overflows
        Assert.Throws<DivideByZeroException>(() => FullMath.MulDiv(Q128, Q128, 0));

        // Revert if the output overflows uint256
        Assert.Throws<OverflowException>(() => FullMath.MulDiv(Q128, Q128, 1));

        // Reverts on overflow with all max inputs
        Assert.ThrowsAny<OverflowException>(() => FullMath.MulDiv(UInt256.MaxValue, UInt256.MaxValue, UInt256.MaxValue - 1));
        
        // All max inputs
        Assert.Equal(FullMath.MulDiv(UInt256.MaxValue, UInt256.MaxValue, UInt256.MaxValue), UInt256.MaxValue);
        
        // Accurate without phantom overflow
        var res = FullMath.MulDiv(
            Q128, 
            new UInt256(50) * Q128 / 100,
            new UInt256(150) * Q128 / 100
            );
        Assert.Equal(res, Q128 / 3);

        // Accurate with phantom overflow
        res = FullMath.MulDiv(Q128, 35 * Q128, 8 * Q128);
        Assert.Equal(res, new UInt256(4375) * Q128 / 1000);

        // Accurate with phantom overflow and repeating decimal
        res = FullMath.MulDiv(Q128, new UInt256(1000) * Q128, (new UInt256(3000) * Q128));
        Assert.Equal(res, Q128 / 3);
    }
}