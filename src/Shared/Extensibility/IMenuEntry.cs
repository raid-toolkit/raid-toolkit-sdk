using System;
using System.ComponentModel;

namespace Raid.Toolkit.Extensibility;

public interface IMenuEntry
{
	string Id { get; set; }
	string DisplayName { get; }
	string? ImageUrl { get; }
	bool IsEnabled { get; }
	bool IsVisible { get; }
	void OnActivate();
}

public class MenuEntry : IMenuEntry
{
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use parameterized constructor")]
	public MenuEntry()
	{
		Id = Guid.NewGuid().ToString("n");
		DisplayName = string.Empty;
	}

	public MenuEntry(string displayName)
	{
		Id = Guid.NewGuid().ToString("n");
		DisplayName = displayName;
	}

	public MenuEntry(string id, string displayName, bool isEnabled, bool isVisible, string? imageUrl)
	{
		Id = id;
		DisplayName = displayName;
		IsEnabled = isEnabled;
		IsVisible = isVisible;
		ImageUrl = imageUrl;
	}

	public event EventHandler? Activate;
	public string Id { get; set; }
	public string DisplayName { get; set; }
	public string? ImageUrl { get; set; }
	public bool IsEnabled { get; set; }
	public bool IsVisible { get; set; }
	public void OnActivate()
	{
		Activate?.Invoke(this, new());
	}
}
