using CommunityToolkit.WinUI.Notifications;

namespace Raid.Toolkit.Extensibility.Notifications;

public class ToastNotification : INotification
{
	private readonly string Title;
	private readonly string Message;
	private readonly string Action;
	public string ScenarioId { get; set; } = string.Empty;

	public ToastContentBuilder ContentBuilder { get; private set; } = new();

	public ToastNotification(string title, string message, string action)
	{
		Title = title;
		Message = message;
		Action = action;
		ContentBuilder
			.AddArgument(NotificationConstants.ScenarioId, ScenarioId)
			.AddArgument(NotificationConstants.Action, Action)
			.AddText(Title)
			.AddText(Message);
	}

	public string GetXml()
	{
		return ContentBuilder.Content.GetContent();
	}
}
