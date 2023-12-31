using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Raid.Toolkit.ExtensionHost.ViewModel;

namespace Raid.Toolkit.ExtensionHost;

public sealed partial class InstallExtensionUI : UserControl
{
	public InstallExtensionUI()
	{
		this.InitializeComponent();
	}

	private void InstallButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		Model.UserTrustResponse(true);
	}

	private void CancelButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		Model.UserTrustResponse(false);
	}

	private void TrustButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		InstallButton.IsEnabled = true;
		Model.ReduceWindowHeight(SecurityWarningBar.ActualHeight);
		SecurityWarningBar.IsOpen = false;
	}

	public ExtensionHostModel Model
	{
		get { return (ExtensionHostModel)GetValue(ModelProperty); }
		set { SetValue(ModelProperty, value); }
	}

	public static readonly DependencyProperty ModelProperty =
		DependencyProperty.Register(nameof(Model), typeof(InstallExtensionUI), typeof(MainPage), new PropertyMetadata(default(ExtensionHostModel)));
}
