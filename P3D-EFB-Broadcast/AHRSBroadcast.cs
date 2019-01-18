using System;
using System.Net.Sockets;
using System.Text;

namespace P3DEFBBroadcast
{
    class AHRSBroadcast
    {
        private UdpClient udpClient = null;
        private string endpoint = null;
        private int port = 49002;

        public AHRSBroadcast(string endpoint = "255.255.255.255", int port = 49002)
        {
            udpClient = new UdpClient();
            this.endpoint = endpoint;
            this.port = port;
        }

        private void SendData(string data)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(data);
            udpClient.Send(bytes, bytes.Length, this.endpoint, this.port);
            Console.WriteLine(data);
        }

        public void BroadcastAircraftData(double latitude, double longitude, double altitude, double groundTrack, double groundSpeed)
        {
            string dataString = $"XGPSPrepar3D,{longitude},{latitude},{altitude},{groundTrack},{groundSpeed}";
            SendData(dataString);
        }

        public void BroadcastTrafficData(int icaoAddress, double latitude, double longitude, double altitude, double verticalSpeed, bool airborne, double groundTrack, double groundSpeed, string callsign)
        {
            int airborneFlag = airborne ? 1 : 0;
            string dataString = $"XGPSPrepar3D,{icaoAddress},{latitude},{longitude},{altitude},{verticalSpeed},{airborneFlag},{groundTrack},{groundSpeed},{callsign}";
            SendData(dataString);
        }

        public void BroadcastAttitudeData(double trueHeading, double pitch, double roll)
        {
            string dataString = $"XGPSPrepar3D,{trueHeading},{pitch},{roll}";
            SendData(dataString);
        }

        ~AHRSBroadcast()
        {
            udpClient.Close();
        }
    }
}
