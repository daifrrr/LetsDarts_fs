namespace Shared.Helpers
module UsefulMaths =
    // https://math.stackexchange.com/questions/1387033/formula-for-alternating-sequences
    let PingPong a b n =
        (((a + b) / 2. ) + ((-1.**(n - 1.)) * ((a - b) / 2.)))
