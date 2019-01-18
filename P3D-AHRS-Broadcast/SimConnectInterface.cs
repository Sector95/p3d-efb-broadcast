using LockheedMartin.Prepar3D.SimConnect;
using System.Runtime.InteropServices;
using System;

namespace P3DAHRSBroadcast
{
    class SimConnectInterface
    {
        private SimConnect simConnect = null;
        private const int WM_USER_SIMCONNECT = 0x0402;

        private OnConnect clientConnectHandler = null;
        private OnDisconnect clientDisconnectHandler = null;
        private OnException clientExceptionHandler = null;
        private OnRecieveAircraftData clientRecieveAircraftDataHandler = null;

        private enum DEFINITIONS
        {
            AircraftData,
            TrafficData
        }

        private enum REQUESTS
        {
            AircraftRequest,
            TrafficRequest
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct AircraftData
        {
            // this is how you declare a fixed size string
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string title;
            public double latitude;
            public double longitude;
            public double altitude;
            public double track;
            public double speed;
        };

        public delegate void OnConnect();
        public delegate void OnDisconnect();
        public delegate void OnException(uint exception);
        public delegate void OnRecieveAircraftData(AircraftData data);

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

        private void OnRecieveAircraftDataHandler(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            switch ((REQUESTS) data.dwRequestID)
            {
                case REQUESTS.AircraftRequest:
                    foreach (AircraftData aircraftData in data.dwData) { clientRecieveAircraftDataHandler(aircraftData); }
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

        public void CheckForMessages()
        {
            simConnect.ReceiveMessage();
        }

        public void InitAircraftDataRequest(OnRecieveAircraftData clientRecieveAircraftData)
        {
            this.clientRecieveAircraftDataHandler = new OnRecieveAircraftData(clientRecieveAircraftData);

            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "TITLE", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS POSITION LAT", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS POSITION LON", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS POSITION ALT", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS GROUND TRUE TRACK", "radians", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
            simConnect.AddToDataDefinition(DEFINITIONS.AircraftData, "GPS GROUND SPEED", "meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

            simConnect.RegisterDataDefineStruct<AircraftData>(DEFINITIONS.AircraftData);
            
            simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecieveAircraftDataHandler);
        }

        public void RequestAircraftData()
        {
            simConnect.RequestDataOnSimObjectType(REQUESTS.AircraftRequest, DEFINITIONS.AircraftData, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
        }
    }
}
