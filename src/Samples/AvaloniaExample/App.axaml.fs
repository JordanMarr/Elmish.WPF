namespace AvaloniaExample

open Avalonia
open Avalonia.Markup.Xaml
open AvaloniaExample.Views

type App() =
    inherit Application()

    override this.Initialize() =
        AvaloniaXamlLoader.Load(this)

    override this.OnFrameworkInitializationCompleted() =
        // Start app only if not IsDesignMode (messes up axaml designer preview)
        if not Avalonia.Controls.Design.IsDesignMode
        then MainView() |> ViewModels.MainViewModel.run

        base.OnFrameworkInitializationCompleted()
