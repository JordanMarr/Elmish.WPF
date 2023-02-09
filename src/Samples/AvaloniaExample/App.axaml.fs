namespace AvaloniaExample

open Avalonia
open Avalonia.Markup.Xaml
open AvaloniaExample.Views

type App() =
    inherit Application()

    override this.Initialize() =        
        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        MainWindow() |> ViewModels.MainWindowViewModel.run
        base.OnFrameworkInitializationCompleted()
