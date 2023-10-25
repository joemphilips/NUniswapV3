module uniswap_v3_math.swap_math

open System
open CustomInt256
open uniswap_v3_math.utils

type private SwapStepResult =
    { sqrtRatioNextX96: uint160
      amountIn: UInt256
      amountOut: UInt256
      feeAmount: UInt256 }

let compute_swap_step
    (
        sqrt_ratio_current_x_96: UInt256,
        sqrt_ratio_target_x_96: UInt256,
        liquidity: UInt128,
        amount_remaining: UInt256,
        fee_pips: uint32
    ) : Result<SwapStepResult, UniswapV3Error> =
    let zeroForOne = sqrt_ratio_current_x_96 >= sqrt_ratio_target_x_96
    let exact_in = amount_remaining >= UInt256.Zero
    let mutable sqrtRatioNextX96 = UInt256.Zero
    let mutable amountIn = UInt256.Zero
    let mutable amountOut = UInt256.Zero

    if exact_in then
        let amountRemainingLessFee = UInt256.
        ()
    else
        ()
