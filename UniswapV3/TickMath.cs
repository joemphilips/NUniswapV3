using System.Globalization;
using CustomInt256;

namespace UniswapV3;

/// <summary>
/// Math library for computing sqrt prices from ticks and vice versa
/// computes sqrt price for ticks of size 1.0001, i.e. sqrt(1.0001^tick)
/// as fixed point Q64.96 numbers. Supports prices between 2**-128 and 2**128
/// </summary>
public static class TickMath
{
    public const int MIN_TICK = -887272;
    public const int MAX_TICK = -MIN_TICK;
    /// <summary>
    /// The minimum value that can be returned from `GetSqrtRatioAtTick`. Equivalent to `GetSqrtRatioAtTick(MIN_TICK)`
    /// </summary>
    public static UInt256 MIN_SQRT_RATIO = new UInt256(4295128739, 0, 0, 0);
    /// <summary>
    /// The maximum value that can be returned from `GetSqrtRatioAtTick`. Equivalent to GetSqrtRatioAtTick(MAX_TICK)
    /// </summary>
    public static UInt256 MAX_SQRT_RATIO = new UInt256(6743328256752651558, 17280870778742802505, 4294805859, 0);

    public static Int256 SQRT_10001 = new Int256(new UInt256(11745905768312294533, 13863, 0, 0));
    public static Int256 TICK_LOW = new Int256(new UInt256(6552757943157144234, 184476617836266586, 0, 0));
    public static Int256 TICK_HIGH = new Int256(new UInt256(4998474450511881007, 15793544031827761793, 0, 0));


    /// <summary>
    /// Calculates sqrt(1.0001^tick) * 2^96
    /// Throws if |tick| > MAX_TICK
    /// </summary>
    /// <param name="tick"></param>
    /// <returns>A Fixed point Q64.96 number representing the sqrt of the ratio of the two assets (token1/token0) at the given tick</returns>
    /// <exception cref="ArgumentException"></exception>
    public static UInt256 GetSqrtRatioAtTick(int tick)
    {
        var absTick = tick < 0 ? (UInt256)(-tick) : (UInt256)tick;
        if (absTick > new UInt256(MAX_TICK))
        {
            throw new ArgumentException($"The given tick must be less than or equal to {MAX_TICK}");
        }

        var ratio = (absTick & (UInt256)0x1) != 0
            ? UInt256.Parse("fffcb933bd6fad37aa2d162d1a594001", NumberStyles.HexNumber)
            : UInt256.Parse("100000000000000000000000000000000", NumberStyles.HexNumber);
     if (!(absTick & (UInt256)0x2).IsZero)
     {
         ratio = (ratio * UInt256.Parse("fff97272373d413259a46990580e213a", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x4).IsZero)
     {
         ratio = (ratio * UInt256.Parse("fff2e50f5f656932ef12357cf3c7fdcc", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x8).IsZero)
     {
         ratio = (ratio * UInt256.Parse("ffe5caca7e10e4e61c3624eaa0941cd0", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x10).IsZero)
     {
         ratio = (ratio * UInt256.Parse("ffcb9843d60f6159c9db58835c926644", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x20).IsZero)
     {
         ratio = (ratio * UInt256.Parse("ff973b41fa98c081472e6896dfb254c0", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x40).IsZero)
     {
         ratio = (ratio * UInt256.Parse("ff2ea16466c96a3843ec78b326b52861", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x80).IsZero)
     {
         ratio = (ratio * UInt256.Parse("fe5dee046a99a2a811c461f1969c3053", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x100).IsZero)
     {
         ratio = (ratio * UInt256.Parse("fcbe86c7900a88aedcffc83b479aa3a4", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x200).IsZero)
     {
         ratio = (ratio * UInt256.Parse("f987a7253ac413176f2b074cf7815e54", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x400).IsZero)
     {
         ratio = (ratio * UInt256.Parse("f3392b0822b70005940c7a398e4b70f3", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x800).IsZero)
     {
         ratio = (ratio * UInt256.Parse("e7159475a2c29b7443b29c7fa6e889d9", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x1000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("d097f3bdfd2022b8845ad8f792aa5825", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x2000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("a9f746462d870fdf8a65dc1f90e061e5", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x4000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("70d869a156d2a1b890bb3df62baf32f7", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x8000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("31be135f97d08fd981231505542fcfa6", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x10000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("9aa508b5b7a84e1c677de54f3e99bc9", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x20000).IsZero) {
         ratio = (ratio * UInt256.Parse("5d6af8dedb81196699c329225ee604", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x40000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("2216e584f5fa1ea926041bedfe98", NumberStyles.HexNumber)) >> 128;
     }
     if (!(absTick & (UInt256)0x80000).IsZero)
     {
         ratio = (ratio * UInt256.Parse("48a170391f7dc42444e8fa2", NumberStyles.HexNumber)) >> 128;
     }

     if (tick > 0) {
         ratio = UInt256.MaxValue / ratio;
     }

     return (ratio >> 32) + (UInt256)((ratio % (UInt256.One << 32)).IsZero ? 0 : 1);
    }

    /// <summary>
    /// Calculates the greatest tick value such that <c>GetRatioAtTick(tick)</c> is less than or equal to <c>ratio</c>
    /// Throws <c>ArgumentException</c> in case of <c>sqrtPriceX96</c> is less than <c>MIN_SQRT_RATIO</c> as
    /// <c>MIN_SQRT_RATIO</c> is the lowest value GetRatioAtTick may ever return
    /// </summary>
    /// <returns></returns>
    public static int GetTickAtSqrtRatio(UInt256 sqrtPriceX96)
    {
        if (!(MIN_SQRT_RATIO <= sqrtPriceX96 && sqrtPriceX96 < MAX_SQRT_RATIO))
            throw new ArgumentException($"MIN_SQRT_RATIO  <= sqrtPriceX96 < MAX_SQRT_RATIO un satisfied (sqrtPriceX96: {sqrtPriceX96})");

        var ratioOrigin = sqrtPriceX96 << 32;
        UInt256 msb = 0;

        int f = ratioOrigin > UInt256.Parse("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", NumberStyles.HexNumber) ? 1 << 7 : 0;
        msb = msb | (UInt256)f;
        var  ratio = ratioOrigin >> f;

        f = ratio > UInt256.Parse("FFFFFFFFFFFFFFFF", NumberStyles.HexNumber) ? 1 << 6 : 0;
        msb = msb | (UInt256)f;
        ratio = ratio >> f;
        
        f = ratio > UInt256.Parse("FFFFFFFF", NumberStyles.HexNumber) ? 1 << 5 : 0;
        msb = msb | (UInt256)f;
        ratio = ratio >> f;
        
        f = ratio > UInt256.Parse("FFFF", NumberStyles.HexNumber) ? 1 << 4 : 0;
        msb = msb | (UInt256)f;
        ratio = ratio >> f;
        
        f = ratio > UInt256.Parse("FF", NumberStyles.HexNumber) ? 1 << 3 : 0;
        msb = msb | (UInt256)f;
        ratio = ratio >> f;
        
        f = ratio > UInt256.Parse("F", NumberStyles.HexNumber) ? 1 << 2 : 0;
        msb = msb | (UInt256)f;
        ratio = ratio >> f;
        
        f = ratio > UInt256.Parse("3", NumberStyles.HexNumber) ? 1 << 1 : 0;
        msb = msb | (UInt256)f;
        ratio = ratio >> f;
        
        f = ratio > UInt256.Parse("1", NumberStyles.HexNumber) ? 1 : 0;
        msb = msb | (UInt256)f;

        ratio = msb >= 128 ? ratioOrigin >> (int)(msb - 127) : ratio << (int)(127 - msb);

        var log2 = (new Int256(msb) - 128) << 64;
        foreach (var i in Enumerable.Range(51, 13))
        {
            ratio = (ratio * ratio) >> 127;
            var ff = ratio >> 128;
            log2 |= new Int256(ff << i);
            ratio >>= f;
        }

        ratio = unchecked(ratio * ratio) >> 127;
        var fff = ratio >> 128;
        var logSqrt10001 = log2 * SQRT_10001;

        var tickLow = ((logSqrt10001 - TICK_LOW) >> 128).ToInt32(null);
        var tickHigh = ((logSqrt10001 + TICK_HIGH) >> 128).ToInt32(null);

        return tickLow == tickHigh ? tickLow : GetSqrtRatioAtTick(tickHigh) <= sqrtPriceX96 ? tickHigh : tickLow;
    }
}