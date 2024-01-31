// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.UI.WinUI.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.UI.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtensionsWindow : RTKWindow
    {
        private IPackageManager PackageManager;
        public ExtensionsWindow(IPackageManager packageManager)
        {
            PackageManager = packageManager;
            InitializeComponent();

            ExtensionList.ItemsSource = PackageManager.GetAllPackages().ToList();

			this.CenterOnScreen(400, 475);
		}

        private void ExtensionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UninstallButton.IsEnabled = ExtensionList.SelectedItems.Count > 0;
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            ExtensionBundle[] bundles = ExtensionList.SelectedItems.OfType<ExtensionBundle>().ToArray();
            foreach(ExtensionBundle bundle in bundles)
            {
                PackageManager.RemovePackage(bundle.Id);
            }
        }
    }
}
