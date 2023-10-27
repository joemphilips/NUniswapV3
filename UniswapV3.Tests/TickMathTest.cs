using CustomInt256;

namespace UniswapV3.Tests;

using static TickMath;

public class TickMathTest
{
    [Fact]
    public void GetSqrtRatioAtTickBounds()
    {
        // the function should return an error if the tick is out of bounds
        Assert.Throws<ArgumentException>(() => GetSqrtRatioAtTick(MIN_TICK - 1));
        Assert.Throws<ArgumentException>(() => GetSqrtRatioAtTick(MAX_TICK + 1));
    }

    [Fact]
    public void GetSqrtRatioAtTickValues()
    {
        Assert.Equal(4295128739UL, GetSqrtRatioAtTick(MIN_TICK));
        Assert.Equal(4295343490UL, GetSqrtRatioAtTick(MIN_TICK + 1));
        Assert.Equal(UInt256.Parse("1461373636630004318706518188784493106690254656249"), GetSqrtRatioAtTick(MAX_TICK - 1));
        Assert.Equal(UInt256.Parse("1461446703485210103287273052203988822378723970342"), GetSqrtRatioAtTick(MAX_TICK));
        
        // checking hard coded values against solidity results
        Assert.Equal(
            GetSqrtRatioAtTick(50),
            UInt256.Parse("79426470787362580746886972461")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(100),
            UInt256.Parse("79625275426524748796330556128")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(250),
            UInt256.Parse("80224679980005306637834519095")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(500),
            UInt256.Parse("81233731461783161732293370115")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(1000),
            UInt256.Parse("83290069058676223003182343270")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(2500),
            UInt256.Parse("89776708723587163891445672585")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(3000),
            UInt256.Parse("92049301871182272007977902845")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(4000),
            UInt256.Parse("96768528593268422080558758223")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(5000),
            UInt256.Parse("101729702841318637793976746270")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(50000),
            UInt256.Parse("965075977353221155028623082916")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(150000),
            UInt256.Parse("143194173941309278083010301478497")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(250000),
            UInt256.Parse("21246587762933397357449903968194344")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(500000),
            UInt256.Parse("5697689776495288729098254600827762987878")
        );
        Assert.Equal(
            GetSqrtRatioAtTick(738203),
            UInt256.Parse("847134979253254120489401328389043031315994541")
        );
    }

    [Fact]
    public void TestGetTickAtSqrtRatio()
    {
        //throws for too low
        Assert.Throws<ArgumentException>( () => GetTickAtSqrtRatio(MIN_SQRT_RATIO - 1));

        //throws for too high
        Assert.Throws<ArgumentException>(() => GetTickAtSqrtRatio(MAX_SQRT_RATIO));

        //ratio of min tick
        Assert.Equal(MIN_TICK, GetTickAtSqrtRatio(MIN_SQRT_RATIO));

        //ratio of min tick + 1
        Assert.Equal(MIN_TICK + 1, GetTickAtSqrtRatio(UInt256.Parse("4295343490")));
    }
}