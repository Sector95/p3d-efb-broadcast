using System;
using System.Net.Sockets;
using System.Text;

namespace P3DEFBBroadcast
{
    class EFBBroadcast
    {
        private UdpClient udpClient;
        private string endpoint;
        private int port = 49002;

        public struct AircraftData
        {
            public double latitude;
            public double longitude;
            public double altitude;
            public double groundTrack;
            public double groundSpeed;
        }

        public struct TrafficData
        {
            public int icaoAddress;
            public double longitude;
            public double latitude;
            public double altitude;
            public double verticalSpeed;
            public int airborne;
            public double groundTrack;
            public double groundSpeed;
            public string callsign;
        }

        public struct AttitudeData
        {
            public double trueHeading;
            public double pitch;
            public double roll;
        }

        public EFBBroadcast(string endpoint = "255.255.255.255", int port = 49002)
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

        public void BroadcastAircraftData(AircraftData aircraftData)
        {
            string dataString = $"XGPSPrepar3D,{aircraftData.longitude:N6},{aircraftData.latitude:N6},{aircraftData.altitude:N4},{aircraftData.groundTrack:N4},{aircraftData.groundSpeed:N4}";
            SendData(dataString);
        }

        public void BroadcastTrafficData(TrafficData trafficData)
        {
            int airborneFlag = trafficData.airborne >= 1 ? 1 : 0;
            string dataString = $"XTRAFFICPrepar3D,{trafficData.icaoAddress},{trafficData.latitude:N3},{trafficData.longitude:N3},{trafficData.altitude:N1},{trafficData.verticalSpeed:N1},{airborneFlag},{trafficData.groundTrack:N1},{trafficData.groundSpeed:N1},{trafficData.callsign}";
            SendData(dataString);
        }

        public void BroadcastAttitudeData(AttitudeData attitudeData)
        {
            string dataString = $"XATTPrepar3D,{attitudeData.trueHeading:N1},{attitudeData.pitch:N1},{attitudeData.roll:N1},0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0";
            SendData(dataString);
        }

        ~EFBBroadcast()
        {
            udpClient.Close();
        }
    }
}
