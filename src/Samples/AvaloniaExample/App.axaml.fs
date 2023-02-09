namespace AvaloniaExample

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml
open AvaloniaExample.ViewModels
open AvaloniaExample.Views
open FSharp.Interop.Dynamic
open System.Dynamic

type MyVM() = 
    member this.Greeting = "Hey!"
    member this.ClickCmd() = printfn "Clicked"

type App() =
    inherit Application()

    override this.Initialize() =        
        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =

        let app = this.ApplicationLifetime :?> IClassicDesktopStyleApplicationLifetime
        app.MainWindow <-
            //MainWindow(DataContext = MainWindowViewModel())
            //MainWindow(DataContext = MyVM())

            Avalonia.Data.Core.ExpressionObserver.PropertyAccessors.Add(new Elmish.Avalonia.AvaloniaDynamicPropertyAccessorPlugin())        // preview4
            //Avalonia.Data.Core.Plugins.BindingPlugins.PropertyAccessors.Add(new Elmish.Avalonia.AvaloniaDynamicPropertyAccessorPlugin())  // preview5

            //let myVM = MyVM()

            //let vm = ExpandoObject()
            //vm?Greeting <- "Test"
            //vm?ClickCmd <- myVM.ClickCmd
            //MainWindow(DataContext = vm)

            let window = MainWindow()
            ViewModels.MainWindowViewModel.start(window)
            let greeting = window.DataContext?Greeting

            window

        base.OnFrameworkInitializationCompleted()
