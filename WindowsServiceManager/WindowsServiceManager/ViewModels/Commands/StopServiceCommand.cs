using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using WindowsServiceManager.Helper;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class StopServiceCommand : BaseCommand
    {
        public StopServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            _ = Task.Factory.StartNew(() =>
             {
                 var ordered = Utility.DependencyOrder(ServiceControllers.Select(c => c.Controller).ToArray());
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

    }
}
