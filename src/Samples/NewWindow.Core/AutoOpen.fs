﻿[<AutoOpen>]
module AutoOpen


let flip f b a = f a b

let map get set f a =
  a |> get |> f |> flip set a


[<RequireQualifiedAccess>]
module Bool =
  open System.Windows
  let toVisibilityCollapsed (b: bool) : Visibility =
    match b with
    | true  -> Visibility.Visible
    | false -> Visibility.Collapsed


[<AutoOpen>]
module InOutModule =

  [<RequireQualifiedAccess>]
  type InOut<'a, 'b> =
    | In of 'a
    | Out of 'b

  [<RequireQualifiedAccess>]
  module InOut =

    let cata (f: 'a -> 'b) (g: 'a0 -> 'b) (inOut: InOut<'a,'a0>) : 'b =
      match inOut with
      | InOut.In  msg -> msg |> f
      | InOut.Out msg -> msg |> g
