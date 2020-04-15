using System.Diagnostics;
using System.Management;

namespace WindowsServiceManager
{
    public static class Utility
    {
        public static Process GetProcessByServiceName(string serviceName)
        {
            string query = $"SELECT ProcessId FROM Win32_Service WHERE Name='{serviceName}'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject obj in searcher.Get())
            {
                uint processId = (uint)obj["ProcessId"];
                Process process = null;
                process = Process.GetProcessById((int)processId);
                if (process != null)
                {
                    return process;
                }
            }
            return null;
        }
    }
}
