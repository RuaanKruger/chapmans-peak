using System.Text;

namespace Chapmans.Peak.Emulator;

/// <summary>
/// In order to correctly establish a connection to the emulator, we need to authenticate the connection
/// by reading the content of the .emulator_console_auth_token located in %USERPROFILE% (Windows) or $HOME (*nix)
/// </summary>
public static class AuthTokenReader
{
    /// <summary>
    /// Read the token file from the user home folder
    /// </summary>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static string GetToken()
    {
        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var tokenPath = Path.Combine(userProfilePath, ".emulator_console_auth_token");

        if (!File.Exists(tokenPath))
        {
            throw new FileNotFoundException($"Unable to locate authentication file : {tokenPath}");
        }
        
        return File.ReadAllText(tokenPath, Encoding.Default);
    }

}