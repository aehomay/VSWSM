using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Windows.Forms.VisualStyles;

namespace WindowsServiceManager.Helper
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

        public static List<ServiceController> DependencyOrder(ServiceController[] controllers)
        {
            var sorted = new List<ServiceController>();
            foreach (var controller in controllers)
            {
                if (controller.DependentServices.Length > 0)
                {
                    DependencyOrder(controller.DependentServices);
                }
                if (!sorted.Exists(c => c.ServiceName.ToUpper().Equals(controller.ServiceName.ToUpper())))
                    sorted.Add(controller);
            }
            return sorted;
        }
        public static IEnumerable<VisualStudioProcess> GetVisualStudioProcesses()
        {
            var processes = Process.GetProcesses().Where(o => o.ProcessName.Contains("devenv"));
            foreach (var process in processes)
            {
                yield return new VisualStudioProcess(process);
            }
        }
    }
}
