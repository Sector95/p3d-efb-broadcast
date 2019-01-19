using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace P3DEFBBroadcast
{
    public partial class MainForm : Form
    {
        private SynchronizationContext context;
        private Thread broadcastThread;
        private delegate void ProcessStatusDelegate(object status);

        private struct ProcessStatusData
        {
            public bool connected;
            public bool error;
            public string message;
        }

        public MainForm()
        {
            this.context = SynchronizationContext.Current;
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(broadcastThread == null) { StartConnection(); }
            else { EndConnection(); }
        }

        private void StartConnection()
        {
            BroadcastProcess.TickRates tickRates = new BroadcastProcess.TickRates
            {
                gps = 1.0f,
                traffic = 1.0f,
                attitude = 0.25f
            };
            ThreadStart threadStart = new ThreadStart(() => BroadcastThread(this.context, ProcessStatus, tickRates));
            broadcastThread = new Thread(threadStart);
            broadcastThread.IsBackground = true;
            broadcastThread.Start();
            connectButton.Text = "Disconnect";
        }

        private void EndConnection()
        {
            if (broadcastThread != null && broadcastThread.IsAlive) { broadcastThread.Abort(); }
            broadcastThread = null;
            connectButton.Text = "Connect";
        }

        private void ProcessStatus(object status)
        {
            ProcessStatusData processStatus = (ProcessStatusData)status;
            if (!processStatus.connected) { EndConnection(); }
            if (processStatus.error) { MessageBox.Show($"Error: {processStatus.message}"); }
        }

        private static void BroadcastThread(SynchronizationContext context, ProcessStatusDelegate callback, BroadcastProcess.TickRates tickRates)
        {
            try
            {
                BroadcastProcess process = new BroadcastProcess(tickRates);
                while (true) {
                    process.Tick();
                    Thread.Sleep(100);
                }
            } catch (ThreadAbortException e) {
                ProcessStatusData status = new ProcessStatusData
                {
                    connected = false,
                    error = false,
                    message = "Broadcast terminated."
                };
                context.Post(new SendOrPostCallback(callback), status);
            } catch (COMException e) {
                ProcessStatusData status = new ProcessStatusData
                {
                    connected = false,
                    error = true,
                    message = "Connection to Prepar3d failed."
                };
                context.Post(new SendOrPostCallback(callback), status);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
