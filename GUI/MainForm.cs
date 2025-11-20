using GUIApp.Comm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIApp
{
    public partial class MainForm : Form
    {
        HeartbeatListener listener;

        public MainForm()
        {
            InitializeComponent();

            listener = new HeartbeatListener("224.0.0.0", 10000);
            listener.Start();

        }

        private void timerUI_Tick(object sender, EventArgs e)
        {
            SimulatorStatus s = new SimulatorStatus();


            while (listener.StatusQueue.TryDequeue(out s))
            {

            }

        }
    }
}
