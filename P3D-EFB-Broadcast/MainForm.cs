using System;
using System.Drawing;
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
        private Bitmap[] statusImages;

        private struct ProcessStatusData
        {
            public bool connected;
            public bool error;
            public string message;
        }

        public MainForm()
        {
            statusImages = new Bitmap[] {
                new Bitmap(Properties.Resources.gps_off),
                new Bitmap(Properties.Resources.gps_on)
            };

            this.context = SynchronizationContext.Current;
            InitializeComponent();
        }

        ~MainForm()
        {
            EndConnection();
            foreach (Bitmap image in statusImages) { image.Dispose(); }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            StartConnection();
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Properties.Resources.iconAttributions, "Information");
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
        }

        private void EndConnection()
        {
            if (broadcastThread != null && broadcastThread.IsAlive) { broadcastThread.Abort(); }
            broadcastThread = null;
        }

        private void SetStatusDisconnected()
        {
            statusPicture.Image = statusImages[0];
            statusText.Text = "Connecting...";
        }

        private void SetStatusConnected()
        {
            statusPicture.Image = statusImages[1];
            statusText.Text = "Connected!";
        }

        private void ProcessStatus(object status)
        {
            ProcessStatusData processStatus = (ProcessStatusData)status;
            if (processStatus.connected) { SetStatusConnected(); }
            else if (!processStatus.connected) { SetStatusDisconnected(); }
        }

        private static void BroadcastThread(SynchronizationContext context, ProcessStatusDelegate callback, BroadcastProcess.TickRates tickRates)
        {
            while (true)
            {
                try
                {
                    BroadcastProcess process = new BroadcastProcess(tickRates);
                    while (true)
                    {
                        process.Tick();
                        ProcessStatusData status = new ProcessStatusData
                        {
                            connected = true,
                            error = false,
                            message = "Connected."
                        };
                        context.Post(new SendOrPostCallback(callback), status);
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
                    return;
                } catch (COMException e) {
                    ProcessStatusData status = new ProcessStatusData
                    {
                        connected = false,
                        error = true,
                        message = "Connection to Prepar3d failed."
                    };
                    context.Post(new SendOrPostCallback(callback), status);
                }

                // We'll try to reconnect once a second.
                Thread.Sleep(1000);
            }
        }
    }
}
