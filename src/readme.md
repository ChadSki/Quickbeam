I've numbered the project folders so that alphabetical order is dependency
order. This made it a little easier for me to keep track of things, and
hopefully for you as well.

## 1.PythonBinding

Loads the Python dll, hosts a Python environment, and deals with Python-C#
interop.

## 2.NimbusSharp

Provides C# bindings to the Nimbus library for editing Halo (which is written
in Python).

## 3.ConsoleTestApp

A basic console application useful for developing NimbusSharp.

## 4.ICSharpCode.TreeView

Imported from the ICSharpCode project, thanks for making it available under a
free license! :D This library provides an excellent TreeView widget that our
GUI makes use of.

## 5.NimbusSharpGUI

GUI-aware objects built on top of NimbusSharp. This layer mediates between the
GUI and the backend. It defines a few XAML controls as well.

## 6.WpfTestApp

A basic WPF application useful for developing WpfTestApp.

## 7.MetroIde

Legacy GUI code. It's mostly been pared down to some themes and resources. I
hope to eliminate it eventually.

## 8.Quickbeam

The flagship itself. Defines the rest of the GUI and window chrome.
