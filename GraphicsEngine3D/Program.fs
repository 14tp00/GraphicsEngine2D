open Window

let WIDTH : int = 1280
let HEIGHT : int = 720
let TITLE : string = "3DGE"

[<EntryPoint>]
let main _ =
    let window = Window(WIDTH, HEIGHT, TITLE, WindowMode.Fixed)
    window.Run()
    0
