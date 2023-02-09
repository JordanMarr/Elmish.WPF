namespace Elmish.Avalonia

open System
open System.Collections.Generic
open System.ComponentModel
open System.Diagnostics.Contracts
open Avalonia.Data
open Avalonia.Data.Core.Plugins
open Avalonia.Utilities

type AvaloniaDynamicPropertyAccessorPlugin() =
    interface IPropertyAccessorPlugin with
        member __.Match(obj, _) =
          //obj :? IDictionary<string, obj> // ExpandoObject
          obj :? IDynamicViewModel

        member __.Start(reference, propertyName) = 
            Contract.Requires<ArgumentNullException>(reference <> null)
            Contract.Requires<ArgumentNullException>(propertyName <> null)
            new Accessor(reference, propertyName)

and Accessor(reference : WeakReference<obj>, property : string) = 
    inherit PropertyAccessorBase()

    let mutable eventRaised = false

    //interface INotifyPropertyChanged with
    //  [<CLIEvent>]
    //  member __.PropertyChanged = PropertyChangedEventHandler(fun sender e -> ())
    //let propertyChanged = EventHandler<PropertyChangedEventArgs>(fun sender e ->
    //  printfn "Property Changed!"
    //)
    let mutable onPropertyChanged : IDisposable option = None

    interface IWeakEventSubscriber<PropertyChangedEventArgs> with
        member __.OnEvent(_, _, e) = 
            if e.PropertyName = property || String.IsNullOrEmpty(e.PropertyName) then
                eventRaised <- true
                __.SendCurrentValue()

    override __.PropertyType =
      __.Value.GetType()

    override __.Value with get() =
        match reference.TryGetTarget() with
        | true, target -> 
            match target with
            | :? IDynamicViewModel as vm ->
                vm.GetMemberByName(property)
            
            | :? IDictionary<string, obj> as o -> // ExpandoObject
                match o.TryGetValue(property) with
                | true, value -> value
                | _ -> null
            | _ -> null
        | _ -> null

    override __.SetValue(value, _) = 
        match reference.TryGetTarget() with 
        | true, target -> 
            match target with
            | :? IDynamicViewModel as vm ->
                vm.SetMemberByName(property, value)
                true
            | :? IDictionary<string, obj> as o -> 
                o.[property] <- value
                true
            | _ -> false
        | _ -> false

    override __.SubscribeCore() = 
        __.SendCurrentValue()
        __.SubscribeToChanges()

    override __.UnsubscribeCore() =
        match reference.TryGetTarget() with
        | true, target ->
            onPropertyChanged |> Option.iter (fun sub -> sub.Dispose())
            //match target with
            //| :? INotifyPropertyChanged as inpc -> 
            //    WeakEventHandlerManager.Unsubscribe(
            //        inpc,
            //        nameof(inpc.PropertyChanged),
            //        (fun sender e -> ()))
            //| _ -> ()                
        | _ -> ()

    member __.SendCurrentValue() = 
        try
            let value = __.Value
            __.PublishValue(value)
        with _ -> ()

    member __.SubscribeToChanges() = 
        match reference.TryGetTarget() with
        | true, target ->
            match target with
            | :? INotifyPropertyChanged as inpc ->
                onPropertyChanged <-
                  inpc.PropertyChanged.Subscribe(fun e ->
                    __.SendCurrentValue()
                  )
                  |> Some
                //WeakEventHandlerManager.Subscribe(
                //    inpc,
                //    nameof(inpc.PropertyChanged),
                //    //propertyChanged)
                //    (fun sender e ->()))
            | _ -> ()
        | _ -> ()

module AppBuilder =

  type Avalonia.AppBuilder with

    /// Uses the Elmish.Avalonia bindings.
    member appBuilder.UseElmishBindings() =
      BindingPlugins.PropertyAccessors.Add(AvaloniaDynamicPropertyAccessorPlugin())
      appBuilder