using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Options;

namespace Raid.Service.UI
{
    public partial class ErrorsWindow : Form
    {
        private readonly ErrorService ErrorService;
        private readonly FileLoggerOptions LoggerSettings;
        private readonly string LogDirectory;
        public ErrorsWindow(ErrorService errorService, IOptions<FileLoggerOptions> loggerSettings)
        {
            InitializeComponent();
            ErrorService = errorService;
            LoggerSettings = loggerSettings.Value;
            LogDirectory = Path.Combine(LoggerSettings.RootPath, LoggerSettings.BasePath);
        }

        private void ReloadData()
        {
            ReloadErrorsList();
            LoadCurrentLog();
        }

        private void ReloadErrorsList()
        {
            listView1.SuspendLayout();
            listView1.Items.Clear();
            foreach (var error in ErrorService.CurrentErrors.Values)
            {
                _ = listView1.Items.Add(new ListViewItem(new string[] { error.Category.ToString(), error.ErrorCode.ToString(), error.TargetDescription, error.ErrorMessage, error.HelpMessage }));
            }

            if (listView1.Items.Count > 0)
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            listView1.ResumeLayout(true);
        }

        private void LoadCurrentLog()
        {
            string[] allFiles = Directory.GetFiles(LogDirectory);
            if (allFiles.Length == 0)
                return;

            errorLog.SuspendLayout();
            errorLog.Text = string.Empty;
            string currentLog = allFiles.Select(file => new { Created = File.GetCreationTimeUtc(file), Path = file }).OrderByDescending(file => file.Created).First().Path;

            {
                using var fs = new FileStream(currentLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var sr = new StreamReader(fs);
                errorLog.AppendText(sr.ReadToEnd());
            }
            errorLog.ResumeLayout(true);
        }

        private void ErrorsWindow_Load(object sender, EventArgs e)
        {
            ReloadErrorsList();
        }

        private void ErrorsWindow_Shown(object sender, EventArgs e)
        {
            LoadCurrentLog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReloadData();
        }
    }
}
