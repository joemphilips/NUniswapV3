using System.Globalization;
using CustomInt256;

namespace UniswapV3.Tests;

public class SwapMathTest
{
    [Fact]
    public void TestComputeSwapStep1()
    {
        // exact amount in that gets capped at price target in one for zero
        var price = UInt256.Parse("79228162514264337593543950336");
        var priceTarget = UInt256.Parse("79623317895830914510639640423");
        var liquidity = (UInt128)2e18;
        var amount = new Int256(UInt256.Parse("1000000000000000000"));
        var fee = 600u;
        var zeroForOne = false;
        var (sqrtP, amountIn, amountOut, feeAmount) =
            SwapMath.ComputeSwapStep(price, priceTarget, liquidity, amount, fee);
        Assert.Equal(UInt256.Parse("79623317895830914510639640423"), sqrtP);

        Assert.Equal(UInt256.Parse("9975124224178055"), amountIn);
        Assert.Equal(UInt256.Parse("5988667735148"), feeAmount);
        Assert.Equal(UInt256.Parse("9925619580021728"), amountOut);

        amount.Convert(out var amountBigInt);
        var beBytes = amountBigInt.ToBytes32(isBigEndian: true);
        Assert.True(amountIn + feeAmount < new UInt256(beBytes, isBigEndian: true));

        var priceAfterWholeInputAmount =
            SqrtPriceMath.GetNextSqrtPriceFromInput(price, liquidity, amountIn, zeroForOne);

        Assert.Equal(sqrtP, priceTarget);
        Assert.True(sqrtP < priceAfterWholeInputAmount);

    }

    [Fact]
    public void TestComputeSwapStep2()
    {
        // exact amount out that gets capped at price target in one for zero
        var price = UInt256.Parse("79228162514264337593543950336");
        var priceTarget = UInt256.Parse("79623317895830914510639640423");
        var liquidity = (UInt128)2e18;
        var amount = -new Int256(UInt256.Parse("1000000000000000000"));
        var fee = 600u;
        var zeroForOne = false;
        
        var (sqrtP, amountIn, amountOut, feeAmount) =
            SwapMath.ComputeSwapStep(price, priceTarget, liquidity, amount, fee);
        
        Assert.Equal(UInt256.Parse("9975124224178055"), amountIn);
        Assert.Equal(UInt256.Parse("5988667735148"), feeAmount);
        Assert.Equal(UInt256.Parse("9925619580021728"), amountOut);

        Assert.True(amountIn + feeAmount < (UInt256)(-amount));

        var priceAfterWholeOutputAmount =
            SqrtPriceMath.GetNextSqrtPriceFromOutput(price, liquidity, (UInt256)(-amount), zeroForOne);

        Assert.Equal(sqrtP, priceTarget);
        Assert.True(sqrtP < priceAfterWholeOutputAmount);
    }

    [Fact]
    public void TestComputeSwapStep3()
    {
        // exact amount in that is fully spent in one for zero
        var price = UInt256.Parse("79228162514264337593543950336");
        var priceTarget = UInt256.Parse("e6666666666666666666666666", NumberStyles.HexNumber);
        var liquidity = (UInt128)2e18;
        var amount = new Int256(UInt256.Parse("1000000000000000000"));
        var fee = 600u;
        var zeroForOne = false;
        var (sqrtP, amountIn, amountOut, feeAmount) =
            SwapMath.ComputeSwapStep(price, priceTarget, liquidity, amount, fee);
        
        Assert.Equal(UInt256.Parse("999400000000000000"), amountIn);
        Assert.Equal(UInt256.Parse("600000000000000"), feeAmount);
        Assert.Equal(UInt256.Parse("666399946655997866"), amountOut);

        Assert.Equal(amountIn + feeAmount, (UInt256)amount);

        var priceAfterWholeInputAmountLessFee =
            SqrtPriceMath.GetNextSqrtPriceFromInput(price, liquidity, (UInt256)(amount - new Int256(feeAmount)), zeroForOne);

        Assert.True(sqrtP < priceTarget);
        Assert.Equal(sqrtP, priceAfterWholeInputAmountLessFee);
    }
    
    [Fact]
    public void TestComputeSwapStep4()
    {
        // exact amount out that is fully received in one for zero
        var price = UInt256.Parse("79228162514264337593543950336");
        var priceTarget = UInt256.Parse("792281625142643375935439503360");
        var liquidity = (UInt128)2e18;
        var amount = new Int256(UInt256.Parse("1000000000000000000")) * Int256.MinusOne;
        var fee = 600u;
        var zeroForOne = false;
        var (sqrtP, amountIn, amountOut, feeAmount) =
            SwapMath.ComputeSwapStep(price, priceTarget, liquidity, amount, fee);
        
        Assert.Equal(UInt256.Parse("2000000000000000000"), amountIn);
        Assert.Equal(UInt256.Parse("1200720432259356"), feeAmount);
        Assert.Equal((UInt256)(-amount), amountOut);

        var priceAfterWholeOutputAmount =
            SqrtPriceMath.GetNextSqrtPriceFromOutput(price, liquidity, (UInt256)(-amount), zeroForOne);

        Assert.True(sqrtP < priceTarget);
        Assert.Equal(sqrtP, priceAfterWholeOutputAmount);
    }
    
    [Fact]
    public void TestComputeSwapStep5()
    {
        //amount out is capped at the desired amount out
        var (sqrtP, amountIn, amountOut, feeAmount) = SwapMath.ComputeSwapStep(
            UInt256.Parse("417332158212080721273783715441582"),
            UInt256.Parse("1452870262520218020823638996"),
            UInt128.Parse("159344665391607089467575320103"),
            Int256.MinusOne,
            1
        );
        
        Assert.Equal(UInt256.One, amountIn);
        Assert.Equal(UInt256.One, feeAmount);
        Assert.Equal(UInt256.One, amountOut);

        Assert.Equal(UInt256.Parse("417332158212080721273783715441581"), sqrtP);
    }
    
    [Fact]
    public void TestComputeSwapStep6()
    {
        // target price of 1 uses partial input amount

        var (sqrtP, amountIn, amountOut, feeAmount) = SwapMath.ComputeSwapStep(
            UInt256.One + UInt256.One,
            UInt256.One,
            1,
            new Int256(UInt256.Parse("3915081100057732413702495386755767")),
            1
        );

        Assert.Equal(
            amountIn,
            UInt256.Parse("39614081257132168796771975168")
        );
        Assert.Equal(
            feeAmount,
            UInt256.Parse("39614120871253040049813")
        );
        Assert.True(
            amountIn + feeAmount
            < UInt256.Parse("3915081100057732413702495386755767")
        );
        Assert.Equal(UInt256.Zero, amountOut);

        Assert.Equal(sqrtP, UInt256.One);
    }
    
    [Fact]
    public void TestComputeSwapStep7()
    {
        // entire input amount taken as fee
        var (sqrtP, amountIn, amountOut, feeAmount) = SwapMath.ComputeSwapStep(
            UInt256.Parse("2413"),
            UInt256.Parse("79887613182836312"),
            UInt128.Parse("1985041575832132834610021537970"),
            10,
            1872
        );

        Assert.Equal(amountIn, UInt256.Zero);
        Assert.Equal(feeAmount, (UInt256)10);
        Assert.Equal(amountOut, UInt256.Zero);
        Assert.Equal(sqrtP, (UInt256)2413);
    }
    
    [Fact]
    public void TestComputeSwapStep8()
    {
        // handles intermediate insufficient liquidity in zero for one exact output case
        var price = UInt256.Parse("20282409603651670423947251286016");
        var priceTarget = price * 11 / 10;
        var liquidity = (UInt128)1024;
        var amountRemaining = (Int256)(-4);
        var fee = 3000u;

        var (sqrtP, amountIn, amountOut, feeAmount) = SwapMath.ComputeSwapStep(
            price, priceTarget, liquidity, amountRemaining, fee
        );

        Assert.Equal(amountOut, UInt256.Zero);
        Assert.Equal(sqrtP, priceTarget);
        Assert.Equal(amountIn, (UInt256)26215);
        Assert.Equal(feeAmount, (UInt256)79);
    }
    
    [Fact]
    public void TestComputeSwapStep9()
    {
        // handles intermediate insufficient liquidity in one for zero exact output case
        var price = UInt256.Parse("20282409603651670423947251286016");
        var priceTarget = price * 9 / 10;
        var liquidity = (UInt128)1024;
        // virtual reserves of zero are only 262144
        // https://www.wolframalpha.com/input/?i=1024+*+%2820282409603651670423947251286016+%2F+2**96%29

        var amountRemaining = (Int256)(263000);
        var fee = 3000u;

        var (sqrtP, amountIn, amountOut, feeAmount) = SwapMath.ComputeSwapStep(
            price, priceTarget, liquidity, amountRemaining, fee
        );

        Assert.Equal(amountOut, (UInt256)26214);
        Assert.Equal(sqrtP, priceTarget);
        Assert.Equal(amountIn, UInt256.One);
        Assert.Equal(feeAmount, UInt256.One);
    }
}