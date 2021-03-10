module Drawing.Point

open Drawing.Interfaces
open System.Runtime.InteropServices
open FSharp.NativeInterop


let ColorAsInt (p: Point) =
    int p.a + 256 * (int p.b + 256 * (int p.g + 256 * int p.r))

let SetColorFromInt (c : int) (pptr: nativeptr<Point>) =
    let mutable p = NativePtr.read pptr
    p.r <- byte (c / (256 * 256 * 256))
    p.g <- byte (c / (256 * 256))
    p.b <- byte (c / 256)
    p.a <- byte c
    NativePtr.write pptr p

