module Ballistics.Plate

open OpenTK
open Ballistics.Node

type Plate(thickness:int, angle:float, height:int, dx:float, dy:float) = 
    let nodes = 
        Array2D.init thickness height (fun x y -> Node(Vector2d(dx+float x*cos angle-float y*sin angle,dy+ float x*sin angle+float y*cos angle),
            Vector2d.Zero,1.,Color(255,255,0,255)))
        |> Seq.cast<Node> 
       

    member val Nodes = nodes with get, set 

    member this.Colors =  
        this.Nodes
        |> Seq.map (fun nd -> nd.temp, nd.pos)
        |> Seq.map (fun (t, pos) -> (log t)/6., pos)
        |> Seq.map (fun (t, pos)-> Color(255,255,0,255), pos)
        //|> Seq.map (fun (t, pos) -> Color(int((sin t)*255.), 0,int ((cos t)*255.), 255), pos)



type Projectile(l:int, d:int, vx:float,vy:float, dx:float,dy:float) = 
    let nodes = 
        Array2D.init l d (fun x y -> Node(Vector2d(float x+dx,float y+dy),Vector2d.Zero,1., Color(255,0,0,255)))
        |> Seq.cast<Node> 
        |> Seq.map(fun node -> node.v <- Vector2d(vx, vy) ; node)

    member val Nodes = nodes with get, set
    member this.Colors = 
        this.Nodes
        |> Seq.map (fun nd -> nd.temp, nd.pos)
        |> Seq.map (fun (t, pos) -> (log t)/6., pos)
        |> Seq.map (fun (t, pos)-> Color(255,0,0,255), pos)
        //|> Seq.map (fun (t, pos) -> Color(int((sin t)*255.), 0,int ((cos t)*255.), 255), pos)

