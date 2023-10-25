using CustomInt256;

namespace UniswapV3.Tests;

public class UnitTest1
{

    [Fact]
    public void TestUInt256()
    {
        
        Assert.Equal(new UInt256((ulong) 21), new UInt256((UInt128)21));
    }
}