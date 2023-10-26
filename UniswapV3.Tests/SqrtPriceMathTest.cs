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
        Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromInput(
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

        // returns the minimum price for max inputs
        var sqrtPrice = SqrtPriceMath.MAX_U160;
        var liquidity = UInt128.MaxValue;
        var maxAmountNoOverflow = UInt256.MaxValue - (new UInt256(liquidity) << 96) / sqrtPrice;

        result = SqrtPriceMath.GetNextSqrtPriceFromInput(sqrtPrice, liquidity, maxAmountNoOverflow, true);
        Assert.Equal(UInt256.One, result);
        
        // Input amount of 0.1 token1
        result = SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e18,
            UInt256.Parse("100000000000000000"),
            false
            );
        Assert.Equal(UInt256.Parse("87150978765690771352898345369"), result);
        
        // Input amount of 0.1 token0
        result = SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e18,
            UInt256.Parse("100000000000000000"),
            true
        );
        Assert.Equal(UInt256.Parse("72025602285694852357767227579"), result);

        // amountIn > type(uint96).Max and zeroForOne = true
        result = SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e19,
            UInt256.Parse("1267650600228229401496703205376"),
            true
            );
        Assert.Equal(UInt256.Parse("624999999995069620"), result);
        
        // can return 1 with enough amountIn and zeroForOne = true
        result = SqrtPriceMath.GetNextSqrtPriceFromInput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1,
            UInt256.MaxValue / 2,
            true
        );
        Assert.Equal(UInt256.One, result);
    }

    [Fact]
    public void TestGetNextSrtPriceFromOutput()
    {
        // fails if the price is zero
        Assert.Throws<ArgumentException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            0,
            1,
            1000000000,
            false
        ));
        
        // fails if liquidity is zero
        Assert.Throws<ArgumentException>(() => 
            SqrtPriceMath.GetNextSqrtPriceFromOutput(
                1,
                0,
                1000000000,
                false
            )
        );
        
        // fails if output amount is exactly the virtual reserves of token0
        var e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("20282409603651670423947251286016"),
            1024,
            4,
            false
        ));
        Assert.Equal("require((product = amount * sqrtPX96) / amount == sqrtPX96 && numerator1 > product)", e.Message);
        
        // fails if output amount is greater than virtual reserves of token0
        e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("20282409603651670423947251286016"),
            1024,
            5,
            false
        ));
        Assert.Equal("require((product = amount * sqrtPX96) / amount == sqrtPX96 && numerator1 > product)", e.Message);
        
        // fails if output amount is greater than virtual reserves of token1
        e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("20282409603651670423947251286016"),
            1024,
            262145,
            true
        ));
        Assert.Equal("Sqrt price is less than or equal to quotient", e.Message);
        
        //fails if output amount is exactly the virtual reserves of token1
        e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("20282409603651670423947251286016"),
            1024,
            262144,
            true
        ));
        Assert.Equal("Sqrt price is less than or equal to quotient", e.Message);
        
        // succeeds if output amount is just less than the virtual
        var result = SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("20282409603651670423947251286016"),
            1024,
            262143,
            true
        );
        Assert.Equal(
            UInt256.Parse("77371252455336267181195264"),
            result
        );
        
        // puzzling echidna test
        e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("20282409603651670423947251286016"),
            1024,
            4,
            false
        ));
        Assert.Equal("require((product = amount * sqrtPX96) / amount == sqrtPX96 && numerator1 > product)", e.Message);
        
        // returns input price if amount in is zero and zeroForOne = true
        result = SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e17,
            UInt256.Zero,
            true
        );
        Assert.Equal(
            UInt256.Parse("79228162514264337593543950336"),
            result
        );

        //returns input price if amount in is zero and zeroForOne = false
        result = SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e17,
            UInt256.Zero,
            false
        );
        
        Assert.Equal(
            UInt256.Parse("79228162514264337593543950336"),
            result
        );

        //output amount of 0.1 token0
        result = SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e18,
            (UInt128)1e17,
            false
        );
        Assert.Equal(
            UInt256.Parse("88031291682515930659493278152"),
            result
        );

        //output amount of 0.1 token1
        result = SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("79228162514264337593543950336"),
            (UInt128)1e18,
            (UInt256)1e17,
            true
        );
        Assert.Equal(
            UInt256.Parse("71305346262837903834189555302"),
            result
        );

        //reverts if amountOut is impossible in zero for one direction
        e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("79228162514264337593543950336"),
            1,
            UInt256.MaxValue,
            true
        ));

        //reverts if amountOut is impossible in one for zero direction
        e = Assert.Throws<OverflowException>(() => SqrtPriceMath.GetNextSqrtPriceFromOutput(
            UInt256.Parse("79228162514264337593543950336"),
            1,
            UInt256.MaxValue,
            false
        ));
        Assert.Equal(
            "require((product = amount * sqrtPX96) / amount == sqrtPX96 && numerator1 > product)",
            e.Message
        );
    }

    [Fact]
    public void TestGetAmount0Delta()
    {
        // returns0 if prices are equal
        var amount0 = SqrtPriceMath._GetAmount0Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("79228162514264337593543950336"),
            1,
            true
            );
        Assert.Equal(amount0, UInt256.Zero);

        // returns 0 if liquidity is 0
        amount0 = SqrtPriceMath._GetAmount0Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("87150978765690771352898345369"),
            0,
            true
        );
        Assert.Equal(amount0, UInt256.Zero);
        
        // returns 0.1 amount1 for price of 1 to 1.21
        amount0 = SqrtPriceMath._GetAmount0Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("87150978765690771352898345369"),
            (UInt128)1e18,
            true
            );
        Assert.Equal(UInt256.Parse("90909090909090910"), amount0);

        var amount0RoundedDown = SqrtPriceMath._GetAmount0Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("87150978765690771352898345369"),
            (UInt128)1e18,
            false
            );
        Assert.Equal(amount0RoundedDown, amount0 - 1);

        // works for prices that overflow
        var amount0Up = SqrtPriceMath._GetAmount0Delta(
            UInt256.Parse("2787593149816327892691964784081045188247552"),
            UInt256.Parse("22300745198530623141535718272648361505980416"),
            (UInt128)1e18,
            true
            );

        var amount0Down = SqrtPriceMath._GetAmount0Delta(
            UInt256.Parse("2787593149816327892691964784081045188247552"),
            UInt256.Parse("22300745198530623141535718272648361505980416"),
            (UInt128)1e18,
            false
            );
        Assert.Equal(amount0Up, amount0Down + 1);
    }

    [Fact]
    public void TestAmount1Delta()
    {
        // returns0 if prices are equal
        var amount1 = SqrtPriceMath._GetAmount1Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("79228162514264337593543950336"),
            1,
            true
            );
        Assert.Equal(amount1, UInt256.Zero);
        
        // returns 0 if liquidity is 0
        amount1 = SqrtPriceMath._GetAmount1Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("87150978765690771352898345369"),
            0,
            true
        );
        Assert.Equal(amount1, UInt256.Zero);
        
        // returns 0.1 amount1 for price of 1 to 1.21
        amount1 = SqrtPriceMath._GetAmount1Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("87150978765690771352898345369"),
            (UInt128)1e18,
            true
            );
        Assert.Equal(UInt256.Parse("100000000000000000"), amount1);

        var amount1RoundedDown = SqrtPriceMath._GetAmount1Delta(
            UInt256.Parse("79228162514264337593543950336"),
            UInt256.Parse("87150978765690771352898345369"),
            (UInt128)1e18,
            false
            );
        Assert.Equal(amount1RoundedDown, amount1 - 1);
    }

    [Fact]
    public void TestSwapComputation()
    {
        var sqrtPrice = UInt256.Parse("1025574284609383690408304870162715216695788925244");
        var liquidity = UInt128.Parse("50015962439936049619261659728067971248");
        var zeroForOne = true;
        var amountIn = new UInt256(406);

        var sqrtQ = SqrtPriceMath.GetNextSqrtPriceFromInput(sqrtPrice, liquidity, amountIn, zeroForOne);
        
        Assert.Equal(UInt256.Parse("1025574284609383582644711336373707553698163132913"), sqrtQ);

        var amount0Delta = SqrtPriceMath._GetAmount0Delta(sqrtQ, sqrtPrice, liquidity, true);
        Assert.Equal((UInt256)406, amount0Delta);
    }
}