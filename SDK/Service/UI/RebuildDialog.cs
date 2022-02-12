using System.Windows.Forms;

namespace Raid.Service.UI
{
    public partial class RebuildDialog : Form
    {
        public RebuildDialog()
        {
            InitializeComponent();
        }
        public RebuildDialog(string gameVersion)
        {
            InitializeComponent();
            label1.Text = $"Raid Toolkit detected a new game version {gameVersion} and needs to rebuild the game model before it can run.\nThis may take up to 2-3 minutes.";
        }

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
    }
}
