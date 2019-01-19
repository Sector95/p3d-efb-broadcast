using LockheedMartin.Prepar3D.SimConnect;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;

namespace P3DEFBBroadcast
{
    class SimConnectInterface
    {
        private SimConnect simConnect;
        private const int WM_USER_SIMCONNECT = 0x0402;

        private OnConnect clientConnectHandler;
        private OnDisconnect clientDisconnectHandler;
        private OnException clientExceptionHandler;
        private OnRecieveAircraftData clientRecieveAircraftDataHandler;
        private OnRecieveTrafficData clientRecieveTrafficDataHandler;
        private OnRecieveAttitudeData clientRecieveAttitudeDataHandler;

        private enum DEFINITIONS
        {
            AircraftData,
            TrafficData,
            AttitudeData
        }

        private enum REQUESTS
        {
            AircraftRequest,
            TrafficRequest,
            AttitudeRequest
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct AircraftData
        {
            public double latitude;
            public double longitude;
            public double altitude;
            public double groundTrack;
            public double groundSpeed;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct TrafficData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string id;
            public double latitude;
            public double longitude;
            // feet
            public double altitude;
            // ft/min
            public double verticalSpeed;
            public int onGround;
            // degrees
            public double track;
            // knots
            public double groundSpeed;
            public int isUser;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct AttitudeData
        {
            public double trueHeading;
            public double pitch;
            public double roll;
        };

        public delegate void OnConnect();
        public delegate void OnDisconnect();
        public delegate void OnException(uint exception);
        public delegate void OnRecieveAircraftData(AircraftData data);
        public delegate void OnRecieveTrafficData(TrafficData data);
        public delegate void OnRecieveAttitudeData(AttitudeData data);

        public SimConnectInterface(OnConnect connectHandler, OnDisconnect disconnectHandler, OnException exceptionHandler)
        {
            this.clientConnectHandler = new OnConnect(connectHandler);
            this.clientDisconnectHandler = new OnDisconnect(disconnectHandler);
            this.clientExceptionHandler = new OnException(exceptionHandler);
        }

        public void Connect()
        {
            simConnect = new SimConnect("AHRS Broadcast", new IntPtr(0), WM_USER_SIMCONNECT, null, 0);

            simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(OnConnectHandler);
            simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnDisconnectHandler);
            simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnExceptionHandler);
        }

        private void OnConnectHandler(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            clientConnectHandler();
        }

        private void OnDisconnectHandler(SimConnect sender, SIMCONNECT_RECV data)
        {
            clientDisconnectHandler();
        }

        private void OnExceptionHandler(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            clientExceptionHandler(data.dwException);
        }

        private void OnRecieveDataHandler(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            switch ((REQUESTS) data.dwRequestID)
            {
                case REQUESTS.AircraftRequest:
                    foreach (AircraftData aircraftData in data.dwData) { clientRecieveAircraftDataHandler(aircraftData); }
                    break;

                case REQUESTS.TrafficRequest:
                    foreach (TrafficData trafficData in data.dwData) { clientRecieveTrafficDataHandler(trafficData); }
                    break;

                case REQUESTS.AttitudeRequest:
                    foreach (AttitudeData attitudeData in data.dwData) { clientRecieveAttitudeDataHandler(attitudeData); }
                    break;

                default:
                    break;
            }
        }

        public void Disconnect()
        {
            if (simConnect != null)
            {
                simConnect.Dispose();
                simConnect = null;
            }
        }

        public bool IsConnected()
        {
            if(simConnect != null) { return true; }
            else { return false; }
        }

        public void RequestAircraftData() { simConnect.RequestDataOnSimObjectType(REQUESTS.AircraftRequest, DEFINITIONS.AircraftData, 0, SIMCONNECT_SIMOBJECT_TYPE.USER); }
        public void RequestTrafficData() { simConnect.RequestDataOnSimObjectType(REQUESTS.TrafficRequest, DEFINITIONS.TrafficData, 92600, SIMCONNECT_SIMOBJECT_TYPE.AIRCRAFT); }
        public void RequestAttitudeData() { simConnect.RequestDataOnSimObjectType(REQUESTS.AttitudeRequest, DEFINITIONS.AttitudeData, 0, SIMCONNECT_SIMOBJECT_TYPE.USER); }

        public void CheckForMessages() { simConnect.ReceiveMessage(); }

        public void InitDataRequest(OnRecieveAircraftData clientRecieveAircraftData, OnRecieveTrafficData clientRecieveTrafficData, OnRecieveAttitudeData clientRecieveAttitudeData)
        {
            this.clientRecieveAircraftDataHandler = clientRecieveAircraftData;
            this.clientRecieveTrafficDataHandler = clientRecieveTrafficData;
            this.clientRecieveAttitudeDataHandler = clientRecieveAttitudeData;

            // Aircraft Data
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS POSITION LAT", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS POSITION LON", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS POSITION ALT", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS GROUND TRUE TRACK", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS GROUND SPEED", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simConnect.RegisterDataDefineStruct<AircraftData>(DEFINITIONS.AircraftData);

            // Traffic Data
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "ATC ID", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "PLANE LATITUDE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "PLANE LATITUDE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "PLANE ALTITUDE", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "VELOCITY WORLD Y", "feet per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "SIM ON GROUND", "boolean", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "PLANE HEADING DEGREES TRUE", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "GROUND VELOCITY", "knots", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.TrafficData, "IS USER SIM", "boolean", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simConnect.RegisterDataDefineStruct<TrafficData>(DEFINITIONS.TrafficData);

            // Attitude Data
            simConnect.AddToDataDefinition(DEFINITIONS.AttitudeData, "PLANE HEADING DEGREES TRUE", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AttitudeData, "PLANE PITCH DEGREES", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AttitudeData, "PLANE BANK DEGREES", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simConnect.RegisterDataDefineStruct<AttitudeData>(DEFINITIONS.AttitudeData);

            simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecieveDataHandler);
        }
    }
}
