module Drawing.EmptyMatrix

open Drawing.Interfaces
open OpenTK

type EmptyMatrix() =
    interface IDisplayMatrix with
        member __.MapPoint _ _ = 
            Vector3d ()
        member __.Refresh () = 
            ()