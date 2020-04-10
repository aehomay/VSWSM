using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class KillServiceCommand : BaseCommand
    {
        const int TIME_OUT_IN_MINUTE = 1;

        public KillServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            var sorted = DependencyOrder(Controllers.Values.ToArray());//TODO: Work on the dependency order method to be more elegent.
            foreach (var controller in sorted)
            {
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    try
                    {
                        KillServiceByProcess(controller.ServiceName);
                        Controllers.Keys.FirstOrDefault(k => k.ServiceName.ToUpper().Equals(controller.ServiceName.ToUpper())).Status = controller.Status;
                    }
                    catch (Exception ex)
                    {
                        ViewMode.ExceptionText = $"Exception happed during the service stop request. " +
                            $"Exception: {ex.Message} InnerException: {ex.InnerException}";
                        ViewMode.Logger.SetLogLevel(Logger.LoggingLevel.Critical).WriteLog(ViewMode.ExceptionText);
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (this)
                {
                    ViewMode.WindowsServiceCollectionView.Refresh();
                }
            });
        }

        private void KillServiceByProcess(string serviceName)
        {
            string query = $"SELECT ProcessId FROM Win32_Service WHERE Name={serviceName}";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject obj in searcher.Get())
            {
                uint processId = (uint)obj["ProcessId"];
                Process process = null;
                process = Process.GetProcessById((int)processId);
                if (process != null)
                {
                    process.Kill();
                    break;
                }
            }
        }

        private IEnumerable<ServiceController> DependencyOrder(ServiceController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.DependentServices.Length > 0)
                    DependencyOrder(controller.DependentServices);
                yield return controller;
            }
        }
    }
}
