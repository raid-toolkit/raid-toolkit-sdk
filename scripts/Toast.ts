import { PowerShell } from 'node-powershell';

export function showToast(title: string, text: string) {
  PowerShell.$`
  [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] > $null
  $Template = [Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::ToastText02)

  $RawXml = [xml] $Template.GetXml()
  ($RawXml.toast.visual.binding.text|where {$_.id -eq "1"}).AppendChild($RawXml.CreateTextNode(${title})) > $null
  ($RawXml.toast.visual.binding.text|where {$_.id -eq "2"}).AppendChild($RawXml.CreateTextNode(${text})) > $null

  $SerializedXml = New-Object Windows.Data.Xml.Dom.XmlDocument
  $SerializedXml.LoadXml($RawXml.OuterXml)

  $Toast = [Windows.UI.Notifications.ToastNotification]::new($SerializedXml)
  $Toast.Tag = "PowerShell"
  $Toast.Group = "PowerShell"
  $Toast.ExpirationTime = [DateTimeOffset]::Now.AddMinutes(1)

  $Notifier = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier("PowerShell")
  $Notifier.Show($Toast);`;
}
