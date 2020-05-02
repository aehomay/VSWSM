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
            sorted = new List<ServiceController>();
            ViewMode.ExceptionText = string.Empty;
            DependencyOrder(Controllers.ToArray());//TODO: Work on the dependency order method to be more elegent.
            _ = Task.Factory.StartNew(() =>
             {
                 foreach (var controller in sorted)
                 {
                     if (controller.Status == ServiceControllerStatus.Running)
                     {
                         try
                         {
                             if (controller.CanStop)
                             {
                                 controller.Stop();
                                //Refresh();//TODO: Improve this part shouldn`t call this for every service.
                                controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(TIME_OUT_IN_MINUTE));
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
