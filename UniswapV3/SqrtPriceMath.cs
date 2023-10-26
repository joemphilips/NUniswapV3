#if DEBUG
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("UniswapV3.Tests")]
#endif

namespace UniswapV3;

using System.Numerics;
using CustomInt256;

public static class SqrtPriceMath
{
    public static UInt256 MAX_U160 = new UInt256(18446744073709551615, 18446744073709551615, 4294967295, 0);
    public static UInt256 Q96 = new UInt256(0, 4294967296, 0, 0);
    public const int FIXED_POINT_96_RESOLUTION = 96;
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
            zeroForOne ? GetNextSqrtPriceFromAmount0RoundingUp(
                sqrtPrice,
                liquidity,
                amountIn,
                true
                ) : GetNextSqrtPriceFromAmount1RoundingDown(
                sqrtPrice,
                liquidity,
                amountIn,
                true
                );
    }
    
    public static UInt256 GetNextSqrtPriceFromOutput(UInt256 sqrtPrice, UInt128 liquidity, UInt256 amountOut, bool zeroForOne) {
        if (sqrtPrice.IsZero)
            throw new ArgumentException("sqrtPrice is zero");
        else if (liquidity == 0)
            throw new ArgumentException("liquidity is zero");

        return zeroForOne ? GetNextSqrtPriceFromAmount1RoundingDown(
                sqrtPrice,
                liquidity,
                amountOut,
                false
            ) : GetNextSqrtPriceFromAmount0RoundingUp(
                sqrtPrice,
                liquidity,
                amountOut,
                false
            );
    }

    private static UInt256 GetNextSqrtPriceFromAmount0RoundingUp(
        UInt256 sqrtPriceX96,
        UInt128 liquidity,
        UInt256 amount,
        bool add
    )
    {
        if (amount.IsZero)
            return sqrtPriceX96;

        Span<byte> span = stackalloc byte[16];
        var numerator1 = new UInt256(liquidity) << 96;

        if (add)
        {
            UInt256.MultiplySaturating(amount, sqrtPriceX96, out var product);

            if (product / amount == sqrtPriceX96)
            {
                UInt256 denom = unchecked(numerator1 + product);
                if (denom >= numerator1)
                {
                    return FullMath.MulDivRoundingUp(numerator1, sqrtPriceX96, denom);
                }
            }

            var b = unchecked(numerator1 / sqrtPriceX96 + amount);
            return UnsafeMath.DivRoundingUp(numerator1, b);
        }
        else
        {
            UInt256.MultiplySaturating(amount, sqrtPriceX96, out var product);
            if (product / amount == sqrtPriceX96 && numerator1 > product)
            {
                var denom = unchecked(numerator1 - product);

                return FullMath.MulDivRoundingUp(numerator1, sqrtPriceX96, denom);
            }
            // If the product overflows, we know the denominator underflows
            // in addition, we must check that the denominator does not underflow
            throw new OverflowException("require((product = amount * sqrtPX96) / amount == sqrtPX96 && numerator1 > product)");
        }
    }

    private static UInt256 GetNextSqrtPriceFromAmount1RoundingDown(
        UInt256 sqrtPriceX96,
        UInt128 liquidity,
        UInt256 amount,
        bool add
    )
    {
        if (add)
        {
            var quotient = amount <= MAX_U160
                ? ((amount << FIXED_POINT_96_RESOLUTION) / liquidity)
                : FullMath.MulDiv(amount, Q96, new UInt256(liquidity));

            var nextSqrtPrice = sqrtPriceX96 + quotient;
            if (nextSqrtPrice > MAX_U160)
            {
                throw new OverflowException("Overflow when casting to U160");
            }
            else
            {
                return nextSqrtPrice;
            }
        } else
        {
            var quotient = amount <= MAX_U160 ?
                UnsafeMath.DivRoundingUp(amount << FIXED_POINT_96_RESOLUTION, liquidity) :
                FullMath.MulDivRoundingUp(amount, Q96, liquidity);

            if (sqrtPriceX96 <= quotient)
                throw new OverflowException("Sqrt price is less than or equal to quotient");
            return sqrtPriceX96 - quotient;
        }
    }

    public static Int256 GetAmount0Delta(
        UInt256 sqrtRatioA_X96,
        UInt256 sqrtRatioB_X96,
        Int128 liquidity
    ) =>
        liquidity < 0
            ? -new Int256(_GetAmount0Delta(sqrtRatioA_X96, sqrtRatioB_X96, (UInt128)(-liquidity), false))
            : new Int256(_GetAmount0Delta(sqrtRatioA_X96, sqrtRatioB_X96, (UInt128)liquidity, true));

    internal static UInt256 _GetAmount0Delta(
        UInt256 sqrtRatioA_X96,
        UInt256 sqrtRatioB_X96,
        UInt128 liquidity,
        bool roundUp
    )
    {
        if (sqrtRatioA_X96 > sqrtRatioB_X96)
            (sqrtRatioA_X96, sqrtRatioB_X96) = (sqrtRatioB_X96, sqrtRatioA_X96);

        if (sqrtRatioA_X96.IsZero)
            throw new ArgumentException("Sqrt price is zero");

        var numerator1 = new UInt256(liquidity) << 96;
        var numerator2 = sqrtRatioB_X96 - sqrtRatioA_X96;

        return roundUp ?
            UnsafeMath.DivRoundingUp(FullMath.MulDivRoundingUp(numerator1, numerator2, sqrtRatioB_X96), sqrtRatioA_X96) :
            FullMath.MulDiv(numerator1, numerator2, sqrtRatioB_X96) / sqrtRatioA_X96;
    }
    
    public static Int256 GetAmount1Delta(
        UInt256 sqrtRatioA_X96,
        UInt256 sqrtRatioB_X96,
        Int128 liquidity
    ) =>
        liquidity < 0
            ? -new Int256(_GetAmount1Delta(sqrtRatioA_X96, sqrtRatioB_X96, (UInt128)(-liquidity), false))
            : new Int256(_GetAmount1Delta(sqrtRatioA_X96, sqrtRatioB_X96, (UInt128)liquidity, true));


    internal static UInt256 _GetAmount1Delta(
        UInt256 sqrtRatioA_X96,
        UInt256 sqrtRatioB_X96,
        UInt128 liquidity,
        bool roundUp
    )
    {
        if (sqrtRatioA_X96 > sqrtRatioB_X96)
            (sqrtRatioA_X96, sqrtRatioB_X96) = (sqrtRatioB_X96, sqrtRatioA_X96);
        return roundUp ? FullMath.MulDivRoundingUp(liquidity, sqrtRatioB_X96 - sqrtRatioA_X96, Q96) : FullMath.MulDiv(liquidity, sqrtRatioB_X96 - sqrtRatioA_X96, Q96);
    }
}