// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Newtonsoft.Json;

using Raid.Toolkit.Common;
using Raid.Toolkit.Extensibility;
using Raid.Toolkit.Extensibility.DataServices;
using Raid.Toolkit.Extensibility.Host;
using Raid.Toolkit.Extension.Account;
using Raid.Toolkit.UI.WinUI.Base;

using RaidExtractor.Core;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Forms;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;

using WinRT;

using XamlApplication = Microsoft.UI.Xaml.Application;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Raid.Toolkit.UI.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountsWindow : RTKWindow
    {
        private readonly CachedDataStorage<PersistedDataStorage> Storage;
        private readonly PersistedDataStorage PersistedDataStorage;
        private readonly IAccountManager AccountManager;
        private readonly Extractor Extractor;
        private List<AccountData> Accounts = new();
        public AccountsWindow(CachedDataStorage<PersistedDataStorage> storage, PersistedDataStorage persistedDataStorage, IAccountManager accountManager)
        {
            InitializeComponent();

            Storage = storage;
            PersistedDataStorage = persistedDataStorage;
            AccountManager = accountManager;
            Extractor = new();
            LoadAccountList();

            CenterWindowInMonitor();
        }

        private void LoadAccountList()
        {
            Accounts = AccountManager.Accounts.Select(account => new AccountData(account)).ToList();
            AccountList.ItemsSource = Accounts;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not AppBarButton button || button.Tag is not string accountId)
                return;

            if (!AccountManager.TryGetAccount(accountId, out IAccount? account))
                return;
            AccountData data = new(account);
            SaveFileDialog sfd = new()
            {
                FileName = $"{data.Account.Name}.rtk.json",
                DefaultExt = "json"
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(data));
        }

        private async void DumpButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not AppBarButton button || button.Tag is not string accountId)
                return;

            ContentDialog dialog = new();
            dialog.XamlRoot = this.Content.XamlRoot;
            dialog.Style = XamlApplication.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Use legacy format?";
            dialog.PrimaryButtonText = "No";
            dialog.SecondaryButtonText = "Yes, I know what I'm doing";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "The RaidExtractor dump format will be deprecated in a future release. You should begin using the new RTK Account format as soon as possible to continue using account dumps.\n\nWould you like to continue?";

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                return;

            if (!AccountManager.TryGetAccount(accountId, out IAccount? account))
                return;

            AccountData data = new(account);
            AccountDump dump = Extractor.DumpAccount(data, new StaticDataWrapper(account), DateTime.UtcNow);
            SaveFileDialog sfd = new()
            {
                FileName = $"{data.Account.Name}_deprecated.json",
                DefaultExt = "json"
            };
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            File.WriteAllText(sfd.FileName, JsonConvert.SerializeObject(dump));
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                CheckFileExists = true,
                Filter = "RTK Account Files (*.rtk.json)|*.rtk.json"
            };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string accountDataJson = File.ReadAllText(ofd.FileName);
            IAccountData? account = JsonConvert.DeserializeObject<ImportedAccountData>(accountDataJson);
            if (account == null)
            {
                ContentDialog dialog = new();
                dialog.XamlRoot = this.Content.XamlRoot;
                dialog.Style = XamlApplication.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Unable to load file";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = "Account file was not in the expected format";
                await dialog.ShowAsync();
                return;
            }

            // TODO: Create a new AccountExtension interface for import/export
            // new AccountData(Storage, account.Account.Id)
            // {
            //     Account = account.Account,
            //     Academy = account.Academy,
            //     Arena = account.Arena,
            //     Artifacts = account.Artifacts,
            //     Heroes = account.Heroes,
            //     Resources = account.Resources,
            // };
            LoadAccountList();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
