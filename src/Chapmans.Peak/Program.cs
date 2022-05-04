// See https://aka.ms/new-console-template for more information
using Chapmans.Peak.Emulator;
using Chapmans.Peak.Route;

var provider = new KmlFileRouteProvider("chapmans-peak-drive.kml");
var route = provider.GetRoute().ToList();
Console.WriteLine($"Points in route : {route.Count}");

var authToken = AuthTokenReader.GetToken();

using var client = new EmulatorConnection("127.0.0.1", 5554, authToken);
client.Connect();

foreach (var command in route.Select(point => $"geo fix {point.Latitude} {point.Longitude}"))
{
    client.SendCommand(command);
}

client.Disconnect();