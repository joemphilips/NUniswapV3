module uniswap_v3_math.full_math

open System.Numerics
open CustomInt256
open uniswap_v3_math.utils


let mul_div(a: UInt256, b: UInt256, denominator: UInt256): Result<UInt256, UniswapV3Error> =
    
    // 512-bit multiply [prod1 prod0] = a * b
    // Compute the product mod 2**256 and mod 2**256 - 1
    // then use the Chinese Remainder Theorem to reconstruct
    // the 512 bit result. The result is stored in two 256
    // variables such that product = prod * 2**256 + prod0
    let mutable outref = UInt256.MinValue
    let modV = UInt256.MaxValue
    let mm = a.MultiplyMod(&b, &modV, &outref)
    ()