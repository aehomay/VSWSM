using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class StopServiceCommand : BaseCommand
    {
        const int TIME_OUT_IN_MINUTE = 1;
        private List<ServiceController> sorted = null;
        public StopServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            _ = Task.Factory.StartNew(() =>
             {
                 foreach (var controller in Controllers)
                 {
                     if (controller.Status == ServiceControllerStatus.Running)
                     {
                         try
                         {
                             if (controller.Controller.CanStop)
                             {
                                 controller.Controller.Stop();
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
    }
}
