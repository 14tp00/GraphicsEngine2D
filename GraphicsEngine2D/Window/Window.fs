module Window

open OpenTK
open OpenTK.Graphics
open OpenTK.Input
open OpenTK.Graphics.OpenGL
open Drawing.Interfaces
open Drawing.EmptyMatrix

type WindowMode =
    | Fixed
    | Fullscreen
    | Default

type Window(width, height, title, mode) =
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
    let OnRenderFrame e =
        activeMatrix.Refresh ()
        GL.Begin PrimitiveType.Lines
        GL.LineWidth 10.0f
        GL.Color3 Color.Red
        GL.Vertex3 (Vector3d (0.0, 0.0, 0.0))
        GL.Vertex3 focus
        GL.End ()
        GL.Flush ()
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