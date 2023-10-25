using System.Numerics;
using CustomInt256;

namespace UniswapV3;

public static class SqrtPriceMath
{
    public static UInt256 MAX_U160 = new UInt256(18446744073709551615, 18446744073709551615, 4294967295, 0);
    public static UInt256 Q96 = new UInt256(0, 4294967296, 0, 0);
    public static UInt256 FIXED_POINT_96_RESOLUTION = new UInt256(96, 0, 0, 0);
    public static UInt256 GetNextSqrtPriceFromInput(
        UInt256 sqrtPrice,
        UInt128 liquidity,
        UInt256 amountIn,
        bool zeroForOne
        )
    {
        if (sqrtPrice.IsZero)
            throw new ArgumentException("sqrtPrice is zero");
        else if (liquidity == 0)
            throw new ArgumentException("liquidity is zero");

        return
            zeroForOne ? GetNextSqrtPriceFromAmount0roundingDown(
                sqrtPrice,
                liquidity,
                amountIn,
                false
                ) : GetNextSqrtPriceFromAmount0roundingUp(
                sqrtPrice,
                liquidity,
                amountIn,
                false
                );
    }

    private static UInt256 GetNextSqrtPriceFromAmount0roundingUp(
        UInt256 sqrtPriceX96,
        UInt128 liquidity,
        UInt256 amount,
        bool add
    )
    {
        if (amount.IsZero)
            return sqrtPriceX96;

        Span<byte> span = stackalloc byte[16];
        if (!((IBinaryInteger<UInt128>)(liquidity << 96)).TryWriteBigEndian(span, out var count))
            throw new Exception("todo");
        var numerator1 = (BigInteger)new UInt256(span, true);
        var amountB = (BigInteger)amount;
        var sqrtPriceX96BE  = (BigInteger)sqrtPriceX96;

        if (add)
        {
        }
        else {}

        throw new NotImplementedException();
    }

    private static UInt256 GetNextSqrtPriceFromAmount0roundingDown(
        UInt256 sqrtPrice,
        UInt128 liquidity,
        UInt256 amount,
        bool add
    )
    {
        throw new NotImplementedException();
    }
}