using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Raid.Toolkit.ExtensionHost.ViewModel;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.ExtensionHost;
public sealed partial class ProgressUI : UserControl
{
	public ProgressUI()
	{
		this.InitializeComponent();
	}

	public ExtensionHostModel Model
	{
		get { return (ExtensionHostModel)GetValue(ModelProperty); }
		set { SetValue(ModelProperty, value); }
	}

	public static readonly DependencyProperty ModelProperty =
		DependencyProperty.Register(nameof(Model), typeof(InstallExtensionUI), typeof(MainPage), new PropertyMetadata(default(ExtensionHostModel)));

	private void HideButton_Click(object sender, RoutedEventArgs e)
	{
		Model.Hide();
	}

	private void CloseButton_Click(object sender, RoutedEventArgs e)
	{
		Model.Close();
	}
}
