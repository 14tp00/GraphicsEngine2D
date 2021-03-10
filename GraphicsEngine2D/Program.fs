open Window
open Drawing.PixelMap
open Drawing.Interfaces

let WIDTH : int = 1280
let HEIGHT : int = 720
let TITLE : string = "2DGE"

[<EntryPoint>]
let main _ =
    let window = Window(WIDTH, HEIGHT, TITLE, WindowMode.Fixed)
    let matrix = 
        PixelMap (400, 400)
        :> IPixelMap
    window.SetMatrix matrix
    matrix.Points
    |> Array2D.iteri (fun x y _ -> matrix.Points.[x,y].r <- byte (x + y))
    window.Run()
    0
