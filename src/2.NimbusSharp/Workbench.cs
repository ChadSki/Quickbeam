﻿using System;
using System.Collections.ObjectModel;
using PythonBinding;

namespace NimbusSharp
{
    public class Workbench
    {
        public static Workbench Instance { get; private set; } = new Workbench();

        private PyObj nimbus;

        private Workbench()
        {
            PythonInterpreter.Instance.RunSimpleString("import nimbus");
            nimbus = PythonInterpreter.Instance.MainModule["nimbus"];
        }

        public ObservableCollection<HaloMap> Maps { get; private set; } = new ObservableCollection<HaloMap>();

        public HaloMap OpenMap()
        {
            var newMap = new HaloMap(nimbus["HaloMap"]["from_memory"].Call());
            Maps.Add(newMap);
            return newMap;
        }
    }
}
