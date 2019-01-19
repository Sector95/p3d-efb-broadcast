# P3D EFB Broadcast

This tool uses data provided from SimConnect to broadcast to EFB's like Garmin Pilot and ForeFlight using the same protocol that X-Plane uses.

Currently has only been tested on the following platform:
- Prepar3d v3
- Windows 10
- Garmin Pilot on Android

Garmin Pilot doesn't pick up traffic information from this method from what I can tell, so it'd be great to have some testers on other platforms!

## Why?

I was jealous that the X-Plane guys had the capaiblity to broadcast this information to their EFB's, so I set out looking for a way to do it myself.  I found another solution out there, but it required FSUIPC and I like trying to minimize the number of plugins in my sim.

## How to use?

Check the releases tab for the most recent releases.  Once downloaded and unpacked, simply run the executable, and once Prepar3d is running, click Connect!  The data is now being sent to devices on your network via UDB broadcast.
