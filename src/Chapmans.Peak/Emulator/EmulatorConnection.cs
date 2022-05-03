using System.Net;
using System.Net.Sockets;
using System.Text;
using Ardalis.GuardClauses;

namespace Chapmans.Peak.Emulator;

/// <summary>
/// Simple emulator client
/// </summary>
/// <remarks>
/// A full lists of commands can be found here : https://developer.android.com/studio/run/emulator-console
/// </remarks>
public class EmulatorConnection
{
    private readonly string _authToken;
    private readonly Socket _emulatorSocket;
    private readonly IPEndPoint _endPoint;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="authToken"></param>
    public EmulatorConnection(string ip, int port, string authToken)
    {
        Guard.Against.NullOrEmpty(ip, nameof(ip));
        Guard.Against.NegativeOrZero(port, nameof(port));
        Guard.Against.NullOrEmpty(authToken, nameof(authToken));
        
        _authToken = authToken;

        var ipAddress = IPAddress.Parse(ip);
        _endPoint = new IPEndPoint(ipAddress, port);
        _emulatorSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    /// <summary>
    /// Connect to the emulator
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Connect()
    {
        if (_emulatorSocket.Connected)
        {
            throw new InvalidOperationException("Already connected to emulator");
        }
        
        _emulatorSocket.Connect(_endPoint);
        Authenticate();
    }

    /// <summary>
    /// Authenticate the connection
    /// </summary>
    /// <see cref="AuthTokenReader"/>
    private void Authenticate()
    {
        SendCommand($"auth {_authToken}");
    }

    /// <summary>
    /// Disconnect from the emulator
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Disconnect()
    {
        if (!_emulatorSocket.Connected)
        {
            throw new InvalidOperationException("Not connected to emulator");
        }
    }

    /// <summary>
    /// Sends a command to the emulator
    /// </summary>
    /// <param name="command"></param>
    public void SendCommand(string command)
    {
        Guard.Against.NullOrEmpty(command, nameof(command));
        Console.WriteLine(command);
        
        _emulatorSocket.Send(Encoding.ASCII.GetBytes(command + Environment.NewLine));
        ReadResponse();
    }

    /// <summary>
    /// Reads and prints the response from the emulator for the provided command
    /// </summary>
    private void ReadResponse()
    {
        var bytes = new byte[1024];
        int responseLength;
        while ((responseLength = _emulatorSocket.Receive(bytes)) > 0)
        {
            var responseText = Encoding.ASCII.GetString(bytes, 0, responseLength);
            Console.WriteLine($"Response : {responseText}");

            if (_emulatorSocket.Available == 0)
            {
                break;
            }
        }
    }
}