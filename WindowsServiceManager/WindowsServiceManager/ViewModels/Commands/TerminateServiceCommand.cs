using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class TerminateServiceCommand : BaseCommand
    {
        const int TIME_OUT_IN_MINUTE = 1;

        public TerminateServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            var sorted = DependencyOrder(Controllers.Values.ToArray());//TODO: Work on the dependency order method to be more elegent.
            Task.Factory.StartNew(() =>
            {
                foreach (var controller in sorted)
                {
                    if (controller.Status == ServiceControllerStatus.Running || controller.Status == ServiceControllerStatus.StartPending)
                    {
                        try
                        {
                            TerminateServiceByProcess(controller.ServiceName);
                            Thread.Sleep(500);
                        }
                        catch (Exception ex)
                        {
                            ViewMode.ExceptionText = $"Exception happed during the service stop request. " +
                                $"Exception: {ex.Message} InnerException: {ex.InnerException}";
                        }
                        finally
                        {
                           Refresh();//TODO: Improve this part shouldn`t call this for every service.
                        }
                    }
                }
            });
        }

        private void TerminateServiceByProcess(string serviceName)
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
                    process.Kill();
                    break;
                }
            }
        }

        private void Refresh()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (this)
                {
                    ViewMode.RefreshServiceCommand.Execute(null);
                }
            });
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
