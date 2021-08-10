open Window
open Drawing.PixelMap
open Drawing.Interfaces
open OpenTK
open Ballistics.Plate

let WIDTH : int = 900
let HEIGHT : int = 900
let TITLE : string = "2DGE"



[<EntryPoint>]
let main _ =
    let window = Window(WIDTH, HEIGHT, TITLE, WindowMode.Fixed)
    let matrix = 
        PixelMap (400, 400, Color.Black) 
        :> IPixelMap
    window.SetMatrix matrix

    window.Run()
    0
