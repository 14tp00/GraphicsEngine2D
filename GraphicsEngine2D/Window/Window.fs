module Window

open OpenTK
open OpenTK.Graphics
open OpenTK.Input
open OpenTK.Graphics.OpenGL
open Drawing.Interfaces
open Drawing.EmptyMatrix
open Ballistics.Node
open Ballistics.Plate
open System


type WindowMode =
    | Fixed
    | Fullscreen
    | Default

type Window(width, height, title, mode) =
    let plate = Plate(30,Math.PI/3.,130, 250., 150.)
    let projectile = Projectile(50,5,120.,0.,50.,180.)
    let mutable counter = 0    
    let mutable activeMatrix = 
        EmptyMatrix () :> IDisplayMatrix
    let mutable focus = Vector3d()
    let mutable activeRoutine = (fun () -> ())
    let mode = 
        match mode with
        | Default -> GameWindowFlags.Default
        | Fullscreen -> GameWindowFlags.Fullscreen
        | Fixed -> GameWindowFlags.FixedWindow
    let window = 
        new GameWindow(width, height, GraphicsMode.Default, title, mode)
    let width, height = window.Width, window.Height

    let mutable nodes = 
        plate.Nodes 
        |> Seq.toList 
        |> List.append (projectile.Nodes |> Seq.toList) 

    let nm = NodeManager (400, 400)

    let mutable bonds = nm.Initiate nodes

    let OnRenderFrame e =
        activeMatrix.Refresh ()
        
        //GL.Begin PrimitiveType.Lines
        //GL.LineWidth 10.0f
        //GL.Color3 Color.Red
        //GL.Vertex3 (Vector3d (0.0, 0.0, 0.0))
        //GL.Vertex3 focus
        //GL.End ()
        //GL.Flush ()

        let matrix = activeMatrix :?> IPixelMap             
        matrix.Clear()

        


        
        nodes <-
            nodes
            |> List.filter(fun node -> int node.pos.X>0)
            |> List.filter(fun node -> int node.pos.X<matrix.Width)
            |> List.filter(fun node -> int node.pos.Y>0)
            |> List.filter(fun node -> int node.pos.Y<matrix.Height) 

        nodes
        |> Seq.iter (fun node -> 
            matrix.Points.[int node.pos.X,int node.pos.Y].r <- node.Color.R
            matrix.Points.[int node.pos.X,int node.pos.Y].g <- node.Color.G
            matrix.Points.[int node.pos.X,int node.pos.Y].b <- node.Color.B
            matrix.Points.[int node.pos.X,int node.pos.Y].a <- node.Color.A)


        let  img = Drawing.Bitmap(matrix.Width, matrix.Height)
        

        matrix.Points
        |> Array2D.iteri(fun x y p -> img.SetPixel(x,y,Drawing.Color.FromArgb(int p.a,int p.r,int p.g,int p.b)))
        
        
        //let outputFileName = sprintf "../../../renders/image%d.png" counter
        //use memory = new IO.MemoryStream() 

        //use fs = new IO.FileStream(outputFileName, IO.FileMode.Create, IO.FileAccess.ReadWrite)

        //img.Save(memory, Drawing.Imaging.ImageFormat.Png)
        //let bytes = memory.ToArray()
        //fs.Write(bytes, 0, bytes.Length)

        

        let ns, bs = nm.Tick 0.01 (nodes, bonds)

        nodes <- ns
        bonds <- bs

        counter <- counter + 1
        window.SwapBuffers ()
    let OnMouseMove (e : MouseMoveEventArgs) =
        let x, y = (float e.X / float width), (float e.Y / float height)
        focus <- activeMatrix.MapPoint x y
    do window.RenderFrame.Add OnRenderFrame
    do window.MouseMove.Add OnMouseMove

    member public __.Run () = window.Run ()
    member public __.SetMatrix matrix =
        activeMatrix <- matrix
    member public __.ActiveMatrix = activeMatrix
    member public __.SetRoutine routine =
        activeRoutine <- routine