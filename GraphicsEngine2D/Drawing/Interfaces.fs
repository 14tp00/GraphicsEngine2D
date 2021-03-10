module Drawing.Interfaces

open OpenTK
open System.Runtime.CompilerServices

[<Struct>]
type Point = {
    mutable x: float32;
    mutable y: float32;
    mutable r: byte;
    mutable g: byte;
    mutable b: byte;
    mutable a: byte
}

type IDisplayMatrix =
    abstract member Refresh : unit -> unit
    abstract member MapPoint : float -> float -> Vector3d
    
type IPixelMap = 
    inherit IDisplayMatrix
    abstract member Width : int
    abstract member Height : int
    abstract member PointCount : int
    abstract member Points : Point[,]
    abstract member SetR : int -> int -> byte -> unit
    abstract member SetG : int -> int -> byte -> unit
    abstract member SetB : int -> int -> byte -> unit
    abstract member SetA : int -> int -> byte -> unit
    abstract member SetColor : int -> int -> Color -> unit
    abstract member GetPoint : int -> int -> Point