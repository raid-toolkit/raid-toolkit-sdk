using Raid.Toolkit.Extensibility.Host;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Raid.Toolkit.UI
{
    public partial class ExtensionsWindow : Form
    {
        private readonly IExtensionHostController ExtensionHostController;
        public ExtensionsWindow(IExtensionHostController extensionHostController)
        {
            InitializeComponent();
            ExtensionHostController = extensionHostController;
            LoadExtensions();
        }

        private void LoadExtensions()
        {
            IReadOnlyList<IExtensionManagement> extensions = ExtensionHostController.GetExtensions();
            foreach(IExtensionManagement extension in extensions)
            {
                listView1.Items.Add(new ListViewItem(
                    extension.Bundle.Manifest.DisplayName,
                    GetImageIndexForState(extension.State),
                    GetGroupForState(extension.State))
                    {
                        Tag = extension
                    });
            }
        }

        private ListViewGroup? GetGroupForState(ExtensionState state)
        {
            switch (state)
            {
                case ExtensionState.Disabled: return listView1.Groups[2];
                case ExtensionState.Activated: return listView1.Groups[0];
                case ExtensionState.Error: return listView1.Groups[1];
                case ExtensionState.PendingUninstall: return listView1.Groups[3];
                default: return null;
            }
        }

        private int GetImageIndexForState(ExtensionState state)
        {
            switch (state)
            {
                case ExtensionState.Disabled: return 0;
                case ExtensionState.Activated: return 1;
                case ExtensionState.Error: return 2;
                case ExtensionState.PendingUninstall: return 3;
                default: return -1;
            }
        }


        private List<(ListViewItem ListItem, IExtensionManagement Extension)> GetSelectedItems()
        {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return listView1.SelectedItems.Cast<ListViewItem>().Select(item => (item, item.Tag as IExtensionManagement)).ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = GetSelectedItems();
            disableButton.Visible = selectedItems.Any(item => item.Extension.State == ExtensionState.Activated);
            enableButton.Visible = selectedItems.Any(item => item.Extension.State == ExtensionState.Disabled);
            uninstallButton.Visible = selectedItems.Any(item => item.Extension.State != ExtensionState.PendingUninstall);
            if (selectedItems.Count > 0)
                description.Text = selectedItems[0].Extension.Bundle.Manifest.Description;
            else
                description.Text = string.Empty;
        }

        private void disableButton_Click(object sender, EventArgs e)
        {
            var selectedItems = GetSelectedItems();
            foreach (var (item, extension) in selectedItems)
            {
                if (extension.State == ExtensionState.Activated)
                    ExtensionHostController.DisablePackage(extension.Bundle.Id);

                item.ImageIndex = GetImageIndexForState(extension.State);
                item.Group = GetGroupForState(extension.State);
            }
        }

        private void enableButton_Click(object sender, EventArgs e)
        {
            var selectedItems = GetSelectedItems();
            foreach (var (item, extension) in selectedItems)
            {
                if (extension.State == ExtensionState.Disabled)
                    ExtensionHostController.EnablePackage(extension.Bundle.Id);

                item.ImageIndex = GetImageIndexForState(extension.State);
                item.Group = GetGroupForState(extension.State);
            }
        }

        private void uninstallButton_Click(object sender, EventArgs e)
        {
            var selectedItems = GetSelectedItems();
            foreach (var (item, extension) in selectedItems)
            {
                if (extension.State != ExtensionState.PendingUninstall)
                    ExtensionHostController.UninstallPackage(extension.Bundle.Id);

                item.ImageIndex = GetImageIndexForState(extension.State);
                item.Group = GetGroupForState(extension.State);
            }
        }
    }
}
