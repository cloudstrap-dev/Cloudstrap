using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;

namespace Bloxstrap.Utility
{
    internal static class NetworkOptimizer
    {
        public static void SetFastDNS()
        {
            // find all active ethernet and wi-fi adapters
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && 
                              (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                               nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                .Select(nic => nic.Name);

            if (!interfaces.Any()) return;

            // build 1 giant command string divided by "&&"
            // tells cmd to run next command only if the previous one succeeds
            string combinedArgs = "/c ";
            foreach (var name in interfaces)
            {
                combinedArgs += $"netsh interface ipv4 set dns name=\"{name}\" static 1.1.1.1 primary && ";
                combinedArgs += $"netsh interface ipv4 add dns name=\"{name}\" 1.0.0.1 index=2 && ";
            }

            // Just append the flush command at the end. 
            // Since the loop added an extra "&& " at the very end, this works perfectly:
            combinedArgs += "ipconfig /flushdns";

        private static void RunElevatedCmd(string arguments)
        {
            try 
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    Verb = "runas",           // single UAC prompt
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
            catch (Win32Exception)          // handle user declining UAC prompt (pressing "No")
            {
                App.Logger.WriteLine("NetworkOptimizer", "User declined UAC. DNS was not changed.");
            }
        }
    }
}