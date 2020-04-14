using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
            {//TODO: Work on the warnnings related to creation of tasks
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        controller.Value.Refresh();
                        if (controller.Value.Status == ServiceControllerStatus.Stopped)
                        {
                            controller.Value.Start();
                            Refresh();//TODO: Improve this part shouldn`t call this for every service.
                            controller.Value.WaitForStatus(ServiceControllerStatus.Running);
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewMode.ExceptionText = $"Failed starting service {controller.Value.ServiceName}. " +
                            $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                    }
                    finally
                    {
                        Refresh();
                    }
                });
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
    }
}
