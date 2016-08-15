namespace Views

open FsXaml

// Demonstrates technique for adding "code behind" logic
// Use a different name for the base class, then inherit to add code behind
type MainViewBase = XAML<"MainView.xaml">

// Inherited class is MainView, which is referred to/used in MainWindow.xaml directly
type MainView() =
    inherit MainViewBase()

    let showMessage _ =
        System.Windows.MessageBox.Show "You double clicked on Full Name!"
        |> ignore
