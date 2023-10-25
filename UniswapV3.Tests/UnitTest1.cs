using CustomInt256;
using FsCheck.Xunit;

namespace UniswapV3.Tests;

public class UnitTest1
{

    [Property]
    public void TestUInt256(ulong v)
    {
        Assert.Equal(new UInt256(v), new UInt256((UInt128)v));
    }
}