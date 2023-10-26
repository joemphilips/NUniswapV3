using CustomInt256;

namespace UniswapV3;

public record SwapStepResult
{
    public UInt256 SqrtRatioNextX96 { get; init; }
    public UInt256 AmountIn { get; init; }
    public UInt256 AmountOut { get; init; }
    public UInt256 FeeAmount { get; init; }

    public void Deconstruct(out UInt256 sqrtRatioNextX96, out UInt256 amountIn, out UInt256 amountOut, out UInt256 feeAmount) => (sqrtRatioNextX96, amountIn, amountOut, feeAmount) =
        (SqrtRatioNextX96, AmountIn, AmountOut, FeeAmount);
}

/// <summary>
/// Computes the result of a swap within ticks
/// It contains methods for computing the result of a swap within a single tick price range,
/// i.e., a single tick.
/// </summary>
public static class SwapMath
{

    /// <summary>
    /// Computes the result of swapping some amount in, or amount out, given the parameters of the swap
    /// </summary>
    /// <param name="sqrtRatioCurrentX96"></param>
    /// <param name="sqrtRatioTargetX96"></param>
    /// <param name="liquidity"></param>
    /// <param name="amountRemaining"></param>
    /// <param name="feePips"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static SwapStepResult ComputeSwapStep(UInt256 sqrtRatioCurrentX96, UInt256 sqrtRatioTargetX96, UInt128 liquidity,
        Int256 amountRemaining, uint feePips)
    {
        var zeroForOne = sqrtRatioCurrentX96 >= sqrtRatioTargetX96;
        var exactIn = amountRemaining >= Int256.Zero;
        
        UInt256 sqrtRatioNextX96;
        var amountIn = UInt256.Zero;
        var amountOut = UInt256.Zero;

        if (exactIn)
        {
            var amountRemainingLessFee = FullMath.MulDiv(
                (UInt256)amountRemaining,
                new UInt256((uint)1e6 - feePips),
                new UInt256((uint)1e6)
                );
            
            amountIn = zeroForOne ?
                SqrtPriceMath._GetAmount0Delta(sqrtRatioTargetX96, sqrtRatioCurrentX96, liquidity, true) :
                SqrtPriceMath._GetAmount1Delta(sqrtRatioCurrentX96, sqrtRatioTargetX96, liquidity, true);

            if (amountRemainingLessFee >= amountIn)
                sqrtRatioNextX96 = sqrtRatioTargetX96;
            else
                sqrtRatioNextX96 = SqrtPriceMath.GetNextSqrtPriceFromInput(
                    sqrtRatioCurrentX96,
                    liquidity,
                    amountRemainingLessFee,
                    zeroForOne
                );
        }
        else
        {
            amountOut = zeroForOne
                ? SqrtPriceMath._GetAmount1Delta(
                    sqrtRatioTargetX96,
                    sqrtRatioCurrentX96,
                    liquidity,
                    false
                )
                : SqrtPriceMath._GetAmount0Delta(
                    sqrtRatioCurrentX96,
                    sqrtRatioTargetX96, liquidity, false);
            sqrtRatioNextX96 = ((UInt256)(-amountRemaining) >= amountOut)
                ? sqrtRatioTargetX96
                : SqrtPriceMath.GetNextSqrtPriceFromOutput(sqrtRatioCurrentX96, liquidity, (UInt256)(-amountRemaining),
                    zeroForOne);
        }

        var max = sqrtRatioTargetX96 == sqrtRatioNextX96;

        if (zeroForOne)
        {
            if (!max || !exactIn)
            {
                amountIn = SqrtPriceMath._GetAmount0Delta(
                    sqrtRatioNextX96,
                    sqrtRatioCurrentX96,
                    liquidity,
                    true
                    );
            }

            if (!max || exactIn)
            {
                amountOut = SqrtPriceMath._GetAmount1Delta(
                    sqrtRatioNextX96,
                    sqrtRatioCurrentX96,
                    liquidity,
                    false
                    );
            }
        }
        else
        {
            if (!max || !exactIn)
            {
                amountIn = SqrtPriceMath._GetAmount1Delta(
                    sqrtRatioCurrentX96,
                    sqrtRatioNextX96,
                    liquidity,
                    true
                    );
            }

            if (!max || exactIn)
            {
                amountOut = SqrtPriceMath._GetAmount0Delta(
                    sqrtRatioCurrentX96,
                    sqrtRatioNextX96,
                    liquidity,
                    false
                    );
            }
        }

        if (!exactIn && amountOut > (UInt256)(-amountRemaining))
            amountOut = (UInt256)(-amountRemaining);
        
        if (exactIn && sqrtRatioNextX96 != sqrtRatioTargetX96)
        {
            var feeAmount = (UInt256)amountRemaining - amountIn;
            return new SwapStepResult
            {
                SqrtRatioNextX96 = sqrtRatioNextX96,
                AmountIn = amountIn,
                AmountOut = amountOut,
                FeeAmount = feeAmount
            };
        }
        else
        {
            var feeAmount = FullMath.MulDivRoundingUp(
                amountIn,
                new UInt256(feePips),
                new UInt256((uint)1e6 - feePips)
                );
            return new SwapStepResult
            {
                SqrtRatioNextX96 = sqrtRatioNextX96,
                AmountIn = amountIn,
                AmountOut = amountOut,
                FeeAmount = feeAmount
            };
        }
    } 
}