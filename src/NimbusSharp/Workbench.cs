using System;
using System.Collections.ObjectModel;
using PythonBinding;

namespace NimbusSharp
{
    public class Workbench
    {
        public static ObservableCollection<HaloMap> Maps { get; private set; } = new ObservableCollection<HaloMap>();
    }
}
