module Ballistics.Node
open System

open OpenTK

type Node (pos:Vector2d,v:Vector2d,temp:float, color:Color)=
    member val pos = pos with get, set // pozycja
    member val v = v with get, set // prędkość
    member val temp = temp with get, set // temperatura
    member val bonds:Bond list = [] with get, set // lista wiązań
    member val bucket:int*int = (-1,-1) with get, set // wiaderko
    member val Color = color with get, set

and Bond (n1:Node, n2:Node) =
    //let d = (n1.pos-n2.pos).Length
    let d = 1.
    let h1 = n1.GetHashCode()
    let h2 = n2.GetHashCode()
    member __.n1 = n1
    member __.n2 = n2
    member __.hash = 
        if h1>h2 then h2, h1 
        else h1, h2
    member val ToRemove = false with get, set
    member __.OtherN n= 
        if n = n1 then n2
        else n1
    member val D = d
            

type NodeManager (width, height)=
    let mutable buckets = Array2D.init 401 401 (fun _ _ -> List.Empty) 
    member __.Initiate (nodes:Node list) =        
        nodes
        |> Seq.map (fun node -> 
            nodes
            |> Seq.filter (fun node2 -> (node.pos-node2.pos).LengthSquared<=2.)
            |> Seq.filter (fun node2 -> node2 <> node)
            |> Seq.map (fun node2 -> Bond(node, node2)))
        |> Seq.concat
        |> Seq.toList
        |> List.distinctBy(fun bond -> bond.hash)

    member __.Tick (dt:float) (nodes:Node list, bonds:Bond list)= 
    

        buckets <- Array2D.init 401 401 (fun _ _ -> List.Empty) // wyczyszczenie listy wiaderek

        let mutable t1,t2,t3,t4 = 0.,0.,0.,0.
        let mutable stopWatch = System.Diagnostics.Stopwatch.StartNew()

        
        nodes // wypełnienie wiaderek odpowiadającymi nodeami (również przypisanie nodeowi danych wiaderka, w którym jest)
        |> Seq.iter (fun node -> 
            let X,Y = 400/width*(int node.pos.X), 400/height*(int node.pos.Y) 
            buckets.[X,Y] <- node::buckets.[X,Y] 
            node.bucket <- (X,Y))
        stopWatch.Stop()
        printfn "%f" stopWatch.Elapsed.TotalMilliseconds     
      
        nodes 
        |> Seq.iter(fun node -> // iteracja po każdym node
            let X,Y = node.bucket
           
            stopWatch <- System.Diagnostics.Stopwatch.StartNew()
            let bucketNodes = // uzyskanie wszystkich nodeów z wiaderka w którym jest sprawdzany node i wiaderek sąsiednich
                [(-1,-1);(0,-1);(1,-1);
                (-1,0);(0,0);(1,0);
                (-1,1);(0,1);(1,1)]
                |> Seq.map (fun (i,j) -> i+X, j+Y)
                |> Seq.filter (fun (i, j) -> i>=0 && j>=0 && i<400 && j<400)
                |> Seq.map (fun (i, j) -> buckets.[i,j])
                |> Seq.concat 
            stopWatch.Stop()
            t1<-t1+ stopWatch.Elapsed.TotalMilliseconds

            let bonds = // twórz wiązania między bliskimi node
                bucketNodes
                |> Seq.filter (fun node2 -> (node.pos-node2.pos).LengthSquared<=2.)
                |> Seq.filter (fun node2 -> node2 <> node)
                |> Seq.map (fun node2 -> Bond(node, node2))
                |> Seq.toList

            node.bonds <- node.bonds @ bonds |> List.distinctBy(fun bond -> bond.hash) // zapobiegnij tworzeniu podwójnych wiązań
            stopWatch <- System.Diagnostics.Stopwatch.StartNew()
            node.bonds
            |> Seq.iter (fun bond -> 
                if (bond.n1.pos-bond.n2.pos).LengthSquared>3. then bond.ToRemove <- true) // jeśli wiązanie między dwoma nodeami jest dłuższe niż sqrt(3), 
                                                                                            //to oflaguj je do usunięcia 
            stopWatch.Stop()
            t2<-t2+ stopWatch.Elapsed.TotalMilliseconds

            node.bonds <- 
                node.bonds
                |> List.filter(fun bond -> not bond.ToRemove)  // usuń oflagowane wiązania z listy wiązań

            stopWatch <- System.Diagnostics.Stopwatch.StartNew()
            node.v <- 
                bucketNodes
                |> Seq.filter (fun node2 -> (node.pos-node2.pos).LengthSquared<=4.) // jeśli odległość między nodeami jest <= 2, to odsuń je od siebie
                |> Seq.filter (fun node2 -> node <> node2)
                |> Seq.map (fun node2 -> (node.pos-node2.pos))
                |> Seq.map (fun diffv -> diffv/diffv.LengthSquared)
                |> Seq.fold (+) node.v
            stopWatch.Stop()
            t3<-t3+ stopWatch.Elapsed.TotalMilliseconds

            stopWatch <- System.Diagnostics.Stopwatch.StartNew()
            node.v <- 
                node.bonds
                |> Seq.map (fun bond -> (bond.OtherN node).pos-node.pos, bond) // utrzymuj wiązanie na stałej odpegłości
                |> Seq.map (fun (diff, bond) -> diff-(diff.Normalized() * bond.D))
                |> Seq.map (fun diff -> diff*4.)
                |> Seq.fold (+) node.v       
            stopWatch.Stop()
            t4<-t4+ stopWatch.Elapsed.TotalMilliseconds
            )     // koniec iteracji
        printf ", t1 %f, t2 %f, t3 %f, t4 %f\n" t1 t2 t3 t4
        nodes
        |>List.iter(fun node->node.pos<-node.pos+(node.v*dt)) // zmodyfikuj pozycję node za pomocą jego prędkości

        (nodes, bonds)