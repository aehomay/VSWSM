using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

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
            foreach (var sc in ServiceControllers)
            {
                _ = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (sc.Status == ServiceControllerStatus.Stopped)
                        {
                            sc.Controller.Start();
                            sc.UpdateResolution = TimeSpan.FromSeconds(1);
                        }
                        Thread.Sleep(200);
                    }
                    catch (Exception ex)
                    {
                        ViewMode.ExceptionText = $"Failed starting service {sc.ServiceName}. " +
                            $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                    }
                }, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
            }
        }
    }
}
