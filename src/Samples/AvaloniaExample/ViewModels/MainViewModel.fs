module AvaloniaExample.ViewModels.MainViewModel

open System
open Elmish
open Elmish.Avalonia

type Model = 
    {
        Greeting: string
    }

type Msg = 
    | SetGreeting of string
    | OnClicked

let init() = 
    { 
        Greeting = "Hello, world."
    }, Cmd.none

let update (msg: Msg) (model: Model) = 
    match msg with
    | OnClicked -> 
        { model with Greeting = "Clicked." }, Cmd.none
    | SetGreeting greeting ->
        { model with Greeting = greeting }, Cmd.none

let bindings ()  : Binding<Model, Msg> list = [
    // Properties
    "Greeting" |> Binding.twoWay ((fun m -> m.Greeting), SetGreeting)
    "ClickCmd" |> Binding.cmd OnClicked
]

let designVM = ViewModel.designInstance (init() |> fst) (bindings())

let run (view :Avalonia.Controls.Window) = 
    AvaloniaProgram.mkProgram init update bindings
    |> AvaloniaProgram.runWindow view

let start (view: Avalonia.Controls.Window) =    
    AvaloniaProgram.mkProgram init update bindings
    |> AvaloniaProgram.startElmishLoop view

//let create<'View when 'View :> Windows.Controls.UserControl and 'View : (new : unit -> 'View)> () =
//    let view = new 'View()
//    start view
//    view
