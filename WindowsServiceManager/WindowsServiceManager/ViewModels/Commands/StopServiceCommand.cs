using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class StopServiceCommand : BaseCommand
    {
        private List<ServiceController> sorted = new List<ServiceController>();
        public StopServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            _ = Task.Factory.StartNew(() =>
             {
                 var ordered = DependencyOrder(Controllers.Select(c => c.Controller).ToArray());
                 foreach (var controller in ordered)
                 {
                     if (controller.Status == ServiceControllerStatus.Running)
                     {
                         try
                         {
                             if (controller.CanStop)
                             {
                                 controller.Stop();
                                 controller.WaitForStatus(ServiceControllerStatus.Stopped);
                             }
                         }
                         catch (Exception ex)
                         {
                             ViewMode.ExceptionText = $"Exception happed during the service stop request. " +
                                 $"Exception: {ex.Message} InnerException: {ex.InnerException}";
                         }
                     }
                 }
             }, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
        }

        private List<ServiceController> DependencyOrder(ServiceController[] controllers)
        {
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
    }
}
