namespace ViewModels

open FsXaml
open FSharp.ViewModule
open NimbusSharpGUI

type public MainViewModel () =
    inherit ViewModelBase ()
    member x.ExampleField with get() = "hello"
