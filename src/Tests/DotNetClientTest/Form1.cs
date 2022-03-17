using Raid.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNetClientTest
{
	public partial class Form1 : Form
	{
        RaidToolkitClient api;
        public Form1()
		{
			InitializeComponent();
            api = new RaidToolkitClient();
            api.Connect();
            api.RealtimeApi.AccountListUpdated += LogEventHandler;
            api.RealtimeApi.ViewChanged += LogEventHandler;
            api.RealtimeApi.ReceiveBattleResponse += LogEventHandler;
            
        }

        private void LogEventHandler(object? sender, Raid.Toolkit.DataModel.SerializableEventArgs e)
        {
            eventsList.Items.Add(e.EventName);
        }

        private async void refreshView_Click(object sender, EventArgs e)
        {
            var accounts = await api.RealtimeApi.GetConnectedAccounts();
            var accountId = accounts.First().Id;
            var viewInfo = await api.RealtimeApi.GetCurrentViewInfo(accountId);
            viewKeyLabel.Text = $"{viewInfo.ViewKey} ({viewInfo.ViewId})";
        }
    }
}
