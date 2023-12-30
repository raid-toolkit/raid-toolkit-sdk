using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.Host;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using System.Threading.Tasks;

namespace Raid.Toolkit.ExtensionHost;

public sealed partial class InstallExtensionUI : UserControl
{
	private readonly TaskCompletionSource<bool> DialogCompletionSource = new();

	public InstallExtensionUI()
	{
		this.InitializeComponent();
	}

	public Task<bool> RequestPermission()
	{
		return DialogCompletionSource.Task;
	}

	private void InstallButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		DialogCompletionSource.TrySetResult(true);
	}

	private void CancelButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		DialogCompletionSource.TrySetResult(false);
	}

	private void TrustButton_Click(object _, Microsoft.UI.Xaml.RoutedEventArgs e)
	{
		InstallButton.IsEnabled = true;
		MinHeight -= SecurityWarningBar.ActualHeight;
		Height -= SecurityWarningBar.ActualHeight;
		SecurityWarningBar.IsOpen = false;
	}

	public ExtensionBundle Bundle
	{
		get { return (ExtensionBundle)GetValue(BundleProperty); }
		set { SetValue(BundleProperty, value); }
	}

	public static readonly DependencyProperty BundleProperty =
		DependencyProperty.Register(nameof(Bundle), typeof(ExtensionBundle), typeof(InstallExtensionUI), new PropertyMetadata(null));

	public IPackageManager PackageManager
	{
		get { return (IPackageManager)GetValue(PackageManagerProperty); }
		set { SetValue(PackageManagerProperty, value); }
	}

	public static readonly DependencyProperty PackageManagerProperty =
		DependencyProperty.Register(nameof(PackageManager), typeof(IPackageManager), typeof(InstallExtensionUI), new PropertyMetadata(null));
}
