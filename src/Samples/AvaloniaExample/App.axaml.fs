namespace AvaloniaExample

open Avalonia
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml
open AvaloniaExample.ViewModels
open AvaloniaExample.Views
open System.Dynamic

type App() =
    inherit Application()

    override this.Initialize() =        
        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =

        let app = this.ApplicationLifetime :?> IClassicDesktopStyleApplicationLifetime

        app.MainWindow <-
            let window = MainWindow()
            ViewModels.MainWindowViewModel.start(window)
            window

        base.OnFrameworkInitializationCompleted()
