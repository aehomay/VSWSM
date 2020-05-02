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
    public class StartServiceCommand : BaseCommand
    {

        public StartServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            foreach (var controller in Controllers)
            {
                _ = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        controller.Refresh();
                        if (controller.Status == ServiceControllerStatus.Stopped)
                        {
                            controller.Start();
                            controller.WaitForStatus(ServiceControllerStatus.Running);
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewMode.ExceptionText = $"Failed starting service {controller.ServiceName}. " +
                            $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                    }
                }, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
            }
        }
    }
}
