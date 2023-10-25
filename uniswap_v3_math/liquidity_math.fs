module uniswap_v3_math.liquidity_math

open System
open uniswap_v3_math.utils

let add_delta(x: UInt128, y: Int128): Result<UInt128, UniswapV3Error> =
    if y < Int128.Zero then
        try
            let z = UInt128.op_CheckedSubtraction(x, Int128.op_Explicit(-y))
            Ok(z)
        with :? OverflowException ->
            Error(UniswapV3Error.LiquiditySub)
    else
        try
            let z = UInt128.op_CheckedAddition(x, Int128.op_Explicit(y))
            Ok(z)
        with :? OverflowException ->
            Error(UniswapV3Error.LiquidityAdd)
            