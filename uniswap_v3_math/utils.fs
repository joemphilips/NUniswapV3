module uniswap_v3_math.utils

open System
open System.Runtime.CompilerServices
open CustomInt256
open System.Numerics

type UniswapV3Error =
    | DenominatorIsZero
    | ResultIsUInt256Max
    | SqrtPriceIsZero
    | SqrtPriceIsLteQuotient
    | ZeroValue
    | LiquidityIsZero
    | ProductDivAmount
    | DenominatorIsLteProdOne
    | LiquiditySub
    | LiquidityAdd
    | T
    | R
    | SafeCastToUInt160Overflow
    | TickSpacingError
    | MiddlewareError
    
    override this.ToString() =
        match this with
        | DenominatorIsZero -> "Denominator is 0"
        | ResultIsUInt256Max -> "Result is UInt256.MAX"
        | SqrtPriceIsZero -> "Sqrt price is zero"
        | SqrtPriceIsLteQuotient -> "Sqrt price is less than or equal to quotient"
        | ZeroValue -> "Can not get most significant bit or least significant bit on zero value"
        | LiquidityIsZero -> "Liquidity is 0"
        | _ -> raise <| NotImplementedException()

    
type Tick = {
    liquidity_gross: UInt128
    liquidity_net: Int128
    fee_growth_outside_0_x_128: UInt256
    fee_growth_outside_1_x_128: UInt256
    tick_cumulative_outside: UInt256
    seconds_per_liquidity_outside_x_128: UInt256
    seconds_outside: uint32
    initialized: bool
}
