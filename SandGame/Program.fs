// Learn more about F# at http://fsharp.org

open System
open Window
open Drawing.PixelMap
open Drawing.Interfaces
open Drawing.Point
open OpenTK

let WIDTH : int = 1280
let HEIGHT : int = 720
let TITLE : string = "SandGame"

type GrainType =
    | Sand
    | Empty

let grainTypeCode t =
    match t with
    | Sand -> 0xf2d16bff
    | _ -> 0x000000ff

let codeAsGrainType c =
    match c with
    | 0xf2d16bff -> Sand
    | _ -> Empty

type Grain = {x: int; y: int; gType: GrainType}

let retrieveGrainType x y (pxlMap: IPixelMap) =
    let point = pxlMap.GetPoint x y
    point
    |> ColorAsInt
    |> codeAsGrainType

let retrieveGrain x y (pxlMap: IPixelMap) =
    let t = retrieveGrainType x y pxlMap
    match t with
    | Empty -> None
    | _ -> Some {x = x; y = y; gType = t}

[<EntryPoint>]
let main _ =
    let window = Window(WIDTH, HEIGHT, TITLE, WindowMode.Fixed)
    let matrix = 
        PixelMap (WIDTH, HEIGHT, Color.Black)
        :> IPixelMap
    window.SetMatrix matrix
    window.Run()
    0
