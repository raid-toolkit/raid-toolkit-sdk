using System;
using System.Windows.Forms;

namespace Raid.Toolkit.Extensibility
{
    public class WindowOptions
    {
        public Func<Form> CreateInstance { get; set; }
        public bool RememberPosition { get; set; }
        public bool RememberVisibility { get; set; }
    }
}
