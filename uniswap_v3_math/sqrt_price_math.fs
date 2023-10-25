module uniswap_v3_math.sqrt_price_math

open System
open CustomInt256
open uniswap_v3_math.utils

let get_next_sqrt_price_from_amount_0_rounding_up() =
    ()

let get_next_sqrt_price_from_input(
    sqrt_price: UInt256,
    liquidity: UInt128,
    amountIn: UInt256,
    zeroForOne: bool
    ): Result<UInt256, UniswapV3Error> =
    if sqrt_price = UInt256.Zero then
        Result.Error(UniswapV3Error.SqrtPriceIsZero)
    else if liquidity = UInt128.Zero then
        Error(UniswapV3Error.LiquidityIsZero)
    else if zeroForOne then
        get_next_sqrt_price_from_amount_0_rounding_up()
        ()
    else
        ()
    
let get_next_sqrt_price_from_output() =
    ()