using CustomInt256;

namespace UniswapV3.Tests;

public class SqrtPriceMathTest
{
    [Fact]
    public void TestGetNextSqrtPriceFromInput()
    {
        // fails if price is zero
        Assert.Throws<ArgumentException>(() => SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.Zero,
            0,
            new UInt256(100000000000000000),
            false
            ));
        
        // fails if liquidity is zero
        Assert.Throws<ArgumentException>(() => SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.One,
            0,
            new UInt256(100000000000000000),
            true
            ));
        
        // fails if input amount overflows the price
        Assert.Throws<ArgumentException>(() => SqrtPriceMath.GetNextSqrtPriceFromInput(
            SqrtPriceMath.MAX_U160,
            1024,
            new UInt256(1024),
            false
            ));

        // returns input price if amount in is zero and zeroForOne = true
        var result = SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.One,
            1,
            UInt256.Parse(
                "57896044618658097711785492504343953926634992332820282019728792003956564819968"
            ),
            true
        );
        Assert.Equal(result, UInt256.One);

        // returns input price if amount in is zero and zeroForOne = false
        result = SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e17,
            UInt256.Zero,
            true
            );
        Assert.Equal(result, UInt256.Parse("79228162514264337593543950336"));

        var sqrtPrice = SqrtPriceMath.MAX_U160;
        var liquidity = UInt128.MaxValue;
        // var maxAmountNoOverflow = UInt256.Max - (new UInt256(liquidity) << 96) / sqrtPrice);
    }

}