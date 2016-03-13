﻿using Quickbeam.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace Quickbeam.Converters
{
    [ValueConversion(typeof(String), typeof(TemplateKey))]
    public class HierarchyNodeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tagHierarchyNode = (ExplorerNode)value;
            if (tagHierarchyNode.IsFolder)
                return Application.Current.FindResource("HierarchyClosedFolder");

            var ext = (Path.GetExtension(tagHierarchyNode.Name) ?? "").ToLower();
            switch (ext)
            {
                case ".snd!":
                    return Application.Current.FindResource("HierarchySnd!Tag");

                case ".bitm":
                    return Application.Current.FindResource("HierarchyBitmTag");

                case ".mode":
                    return Application.Current.FindResource("HierarchyModeTag");

                default:
                    return Application.Current.FindResource("HierarchyGenericTag");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
