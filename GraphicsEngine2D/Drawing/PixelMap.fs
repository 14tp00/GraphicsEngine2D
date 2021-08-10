module Drawing.PixelMap

open Drawing.Interfaces
open OpenTK
open OpenTK.Graphics.OpenGL
open System.Runtime.InteropServices

let mutable counter = 0

type PixelMap (width, height, initC : Color) =
    let mutable ps = Array2D.init width height (fun x y -> 
        {x = float32 x; y = float32 y; a = initC.A; r = initC.R; g = initC.G; b = initC.B})
    let pMatrix = (0.0, float width, float height, 0.0, 0.0, 1.0)
    
    interface IPixelMap with
        member __.MapPoint x y = 
            let left, right, bottom, top, near, far = pMatrix
            Vector3d ((right - left) * x, (bottom - top) * y, near)
        member __.PointCount = 
            width * height
        member __.GetPoint x y = 
            ps.[x,y]
        member __.Height = 
            height
        member __.Points = 
            ps
        member __.Refresh(): unit = 
            counter <- counter + 1
            GL.Clear (ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit)
            
            GL.MatrixMode MatrixMode.Projection
            GL.LoadIdentity ()
            GL.Ortho pMatrix
            
            GL.MatrixMode MatrixMode.Modelview
            GL.LoadIdentity ()
                
            GL.EnableClientState ArrayCap.VertexArray
            GL.EnableClientState ArrayCap.ColorArray
            
            GL.VertexPointer (2, VertexPointerType.Float, sizeof<Point>, ps)
            let clrs = Marshal.UnsafeAddrOfPinnedArrayElement (ps, 0) + nativeint (2 * (sizeof<float32>))
            GL.ColorPointer (4, ColorPointerType.UnsignedByte, sizeof<Point>, clrs)
            GL.PointSize 1.0f
            let l = width * height * 3 / 2 // TODO The hell is wrong with it?
            GL.Enable EnableCap.ProgramPointSize 
            GL.PointSize 1f
            GL.DrawArrays (PrimitiveType.Points, 0, l)
            GL.DisableClientState ArrayCap.VertexArray
            GL.DisableClientState ArrayCap.ColorArray
        member __.SetA x y a = 
            ps.[x,y].a <- a
        member __.SetB x y b = 
            ps.[x,y].b <- b
        member __.SetColor x y color = 
            ps.[x,y].r <- color.R
            ps.[x,y].b <- color.B
            ps.[x,y].g <- color.G
            ps.[x,y].a <- color.A
        member __.SetG x y g = 
            ps.[x,y].g <- g
        member __.SetR x y r = 
            ps.[x,y].r <- r
        member __.Width: int = width
        member __.Clear () =
            ps <- Array2D.init width height (fun x y -> 
                {x = float32 x; y = float32 y; a = initC.A; r = initC.R; g = initC.G; b = initC.B})