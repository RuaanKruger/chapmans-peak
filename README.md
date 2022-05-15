![](https://raw.githubusercontent.com/RuaanKruger/chapmans-peak/main/assets/logo.png)
# chapmans-peak
## What is this project?
I maintain a very specialized telemetry project and I needed a way to provide routes to it for testing, from the comfort of my desk.

This example console application illustrates the nifty usage of sending GPS positions to our Android emulator. 

## Concepts
### Parse a KML file for route locations
The Route provider (<a href="https://github.com/RuaanKruger/chapmans-peak/blob/main/src/Chapmans.Peak/Route/KmlFileRouteProvider.cs" target="_blank">KmlFileRouteProvider</a>) parses a KML file and return the points.
```csharp
var provider = new KmlFileRouteProvider("chapmans-peak-drive.kml");
var route = provider.GetRoute().ToList();
```

I used Sam Cragg's KML implementation found <a href="https://github.com/samcragg/sharpkml" target="_blank">here</a> to read the locations from the file.

### Get emulator token
When we create the TCP connection to our emulator, we need to provide the contents of the .emulator_console_auth_token file found in %USERPROFILE% (Windows) or $HOME (*nix).

```csharp
var authToken = AuthTokenReader.GetToken();
```

### Connect to the emulator
The emulator listens for connections on ports **5554** to **5585** and accepts connections from **localhost** only.
```csharp
using var client = new EmulatorConnection("127.0.0.1", 5554, authToken);
client.Connect();
```

### Send the new location to the emulator
```csharp
foreach (var command in route.Select(point => $"geo fix {point.Latitude} {point.Longitude}"))
{
    client.SendCommand(command);
}
```

### Further Reading
There are many commands you can send to the emulator this way and the "geo" command used here is just the tip of the iceberg.

All emulator commands can be found here : https://developer.android.com/studio/run/emulator-console

## Special thanks
To https://www.chapmanspeakdrive.co.za/ for maintaining one of the world's most scenic drives and for the usage of their imagery.
