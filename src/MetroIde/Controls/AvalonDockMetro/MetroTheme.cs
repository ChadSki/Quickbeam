﻿/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using Xceed.Wpf.AvalonDock.Themes;

namespace MetroIde.Controls.AvalonDockMetro
{
    public class MetroTheme : Theme
    {
        public override Uri GetResourceUri()
        {
            return new Uri(
                "/Quickbeam;component/Controls/AvalonDockMetro/Theme.xaml",
                UriKind.Relative);
        }
    }
}