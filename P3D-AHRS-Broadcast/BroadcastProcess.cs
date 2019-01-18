using System;

namespace P3DAHRSBroadcast
{
    public class BroadcastProcess
    {
        private SimConnectInterface simConnect = null;
        private AHRSBroadcast ahrsBroadcast = null;
        private double broadcastRateSeconds = 1.0d;
        private DateTime lastBroadcast = DateTime.UtcNow;

        private struct AircraftData
        {
            public double latitude;
            public double longitude;
            public double altitude;
            public double track;
            public double speed;
        }
        private AircraftData aircraftData;

        public BroadcastProcess()
        {
            simConnect = new SimConnectInterface(OnConnectHandler, OnDisconnectHandler, OnExceptionHandler);
            simConnect.Connect();
            simConnect.InitAircraftDataRequest(OnRecieveAircraftData);

            ahrsBroadcast = new AHRSBroadcast();
        }

        public void Tick()
        {
            simConnect.RequestAircraftData();

            simConnect.CheckForMessages();
            TimeSpan delta = DateTime.UtcNow - lastBroadcast;

            if (delta.TotalSeconds > broadcastRateSeconds)
            {
                lastBroadcast = DateTime.UtcNow;
                ahrsBroadcast.BroadcastAircraftData(aircraftData.latitude, aircraftData.longitude, aircraftData.altitude, aircraftData.track, aircraftData.speed);
            }
        }

        public void Terminate()
        {
            simConnect.Disconnect();
        }

        public void OnConnectHandler()
        {
            Console.WriteLine("Connected to SimConnect!");
        }

        public void OnDisconnectHandler()
        {
            Console.WriteLine("Disconnected from SimConnect!");
        }

        public void OnExceptionHandler(uint exceptionNumber)
        {
            
        }

        private void OnRecieveAircraftData(SimConnectInterface.AircraftData aircraftData)
        {
            double vector_degrees = (180 / Math.PI) * aircraftData.track;

            this.aircraftData.latitude = aircraftData.latitude;
            this.aircraftData.longitude = aircraftData.longitude;
            this.aircraftData.altitude = aircraftData.altitude;
            this.aircraftData.track = vector_degrees;
            this.aircraftData.speed = aircraftData.speed;
        }
    }
}
