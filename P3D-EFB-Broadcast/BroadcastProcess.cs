using System;
using System.Collections.Generic;

namespace P3DEFBBroadcast
{
    public class BroadcastProcess
    {
        private SimConnectInterface simConnect;
        private EFBBroadcast efbBroadcast;
        private TickRates tickRates;
        private LastRequest lastRequest;

        // Delta in seconds between data requests.
        public struct TickRates
        {
            public float gps;
            public float traffic;
            public float attitude;
        }

        private struct LastRequest
        {
            public DateTime gps;
            public DateTime traffic;
            public DateTime attitude;
        }

        public BroadcastProcess(TickRates tickRates)
        {
            this.tickRates = tickRates;
            this.lastRequest = new LastRequest
            {
                gps = DateTime.UtcNow,
                traffic = DateTime.UtcNow,
                attitude = DateTime.UtcNow
            };

            simConnect = new SimConnectInterface(OnConnectHandler, OnDisconnectHandler, OnExceptionHandler);
            simConnect.Connect();
            simConnect.InitDataRequest(OnRecieveAircraftData, OnRecieveTrafficData, OnRecieveAttitudeData);

            efbBroadcast = new EFBBroadcast();
        }

        public void Tick()
        {
            DateTime now = DateTime.UtcNow;

            if ((now - lastRequest.gps).TotalSeconds >= tickRates.gps)
            {
                lastRequest.gps = now;
                simConnect.RequestAircraftData();
            }

            if ((now - lastRequest.traffic).TotalSeconds >= tickRates.traffic)
            {
                lastRequest.traffic = now;
                simConnect.RequestTrafficData();
            }

            if ((now - lastRequest.attitude).TotalSeconds >= tickRates.attitude)
            {
                lastRequest.attitude = now;
                simConnect.RequestAttitudeData();
            }

            simConnect.CheckForMessages();
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
            EFBBroadcast.AircraftData efbAircraftData = new EFBBroadcast.AircraftData
            {
                latitude = aircraftData.latitude,
                longitude = aircraftData.longitude,
                altitude = aircraftData.altitude,
                groundTrack = RadiansToDegrees(aircraftData.groundTrack),
                groundSpeed = aircraftData.groundSpeed
            };

            efbBroadcast.BroadcastAircraftData(efbAircraftData);
        }

        private void OnRecieveAttitudeData(SimConnectInterface.AttitudeData attitudeData)
        {
            EFBBroadcast.AttitudeData efbAttitudeData = new EFBBroadcast.AttitudeData
            {
                trueHeading = RadiansToDegrees(attitudeData.trueHeading),
                pitch = RadiansToDegrees(attitudeData.pitch) * -1,
                roll = RadiansToDegrees(attitudeData.roll) * -1,
            };

            efbBroadcast.BroadcastAttitudeData(efbAttitudeData);
        }

        private void OnRecieveTrafficData(SimConnectInterface.TrafficData trafficData)
        {
            if (trafficData.isUser == 1 || trafficData.onGround == 1) { return; }

            EFBBroadcast.TrafficData efbTrafficData = new EFBBroadcast.TrafficData
            {
                icaoAddress = 123,
                longitude = trafficData.longitude,
                latitude = trafficData.latitude,
                altitude = trafficData.altitude,
                // Feet/second to feet/minute conversion.
                verticalSpeed = trafficData.verticalSpeed / 60,
                airborne = trafficData.onGround,
                groundTrack = RadiansToDegrees(trafficData.track),
                groundSpeed = trafficData.groundSpeed,
                callsign = trafficData.id
            };

            efbBroadcast.BroadcastTrafficData(efbTrafficData);
        }

        private double RadiansToDegrees(double radians)
        {
            return (180 / Math.PI) * radians;
        }
    }
}
