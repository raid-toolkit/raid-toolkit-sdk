using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raid.Toolkit
{
    public class ApplicationStartupTask
    {
        public ApplicationStartupTask()
        { }

        public Task Execute(string[] args)
        {
            Application.Run(new Form1());
            return Task.CompletedTask;
        }
    }
}