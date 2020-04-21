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
using WindowsServiceManager.Helper;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class TerminateServiceCommand : BaseCommand
    {
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
            Utility.GetProcessByServiceName(serviceName)?.Kill();
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
