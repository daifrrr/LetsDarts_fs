[<AutoOpen>]
module Shared.Helpers


module Logger =
    /// Log levels.
    let Error = 0
    let Warning = 1
    let Information = 2
    let Debug = 3

    let LevelToString level =
      match level with
        | 0 -> "Error"
        | 1 -> "Warning"
        | 2 -> "Information"
        | 3 -> "Debug"
        | _ -> "Unknown"

    /// The current log level.
    let mutable current_log_level = Debug

    /// The inteface loggers need to implement.
    type ILogger = abstract Log : int -> Printf.StringFormat<'a,unit> -> 'a

    /// Writes to console.
    let ConsoleLogger = {
      new ILogger with
        member _.Log level format =
          Printf.kprintf (printfn "[%s][%A] %s" (LevelToString level) System.DateTime.Now) format
     }

    /// Defines which logger to use.
    let mutable DefaultLogger = ConsoleLogger

    /// Logs a message with the specified logger.
    let logUsing (logger: ILogger) = logger.Log

    /// Logs a message using the default logger.
    let log level message = logUsing DefaultLogger level message

module UsefulMaths =
    // https://math.stackexchange.com/questions/1387033/formula-for-alternating-sequences
    let PingPong a b n =
        (((a + b) / 2. ) + ((-1.**(n - 1.)) * ((a - b) / 2.)))
