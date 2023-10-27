using CustomInt256;

namespace UniswapV3;

public record Tick()
{
    public UInt128 LiquidityGross { get; init; }
    public Int128 LiquidityNet { get; init; }
    public UInt256 FeeGrowthOutside0X128 { get; init; }
    public UInt256 FeeGrowthOutside1X128 { get; init; }
    public UInt256 TickCumulativeOutside { get; init; }
    public UInt256 SecondsPerLiquidityOutsideX128 { get; init; }
    public uint SecondsOutside { get; init; }
    public bool Initialized { get; init; }
};
