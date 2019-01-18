using System;
using System.Threading;
using System.Windows.Forms;

namespace P3DAHRSBroadcast
{
    public partial class MainForm : Form
    {
        
        private Thread broadcastThread = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(broadcastThread == null)
            {
                ThreadStart childRef = new ThreadStart(BroadcastThread);
                broadcastThread = new Thread(childRef);
                broadcastThread.Start();
                connectButton.Text = "Disconnect";
            }
            else
            {
                broadcastThread.Abort();
                broadcastThread = null;
                connectButton.Text = "Connect";
            }
        }

        public static void BroadcastThread()
        {
            try
            {
                BroadcastProcess process = new BroadcastProcess();
                while (true) {
                    process.Tick();
                    Thread.Sleep(250);
                }
            } catch (ThreadAbortException e) {
                Console.WriteLine("Broadcast Thread Aborted.");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
