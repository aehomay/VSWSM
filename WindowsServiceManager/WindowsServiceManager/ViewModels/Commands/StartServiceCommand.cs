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
        const int TIME_OUT_IN_MINUTE = 1;

        public StartServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            ViewMode.ExceptionText = string.Empty;
            var tasks = new List<Task>();
            foreach (var controller in Controllers)
            {//TODO: Work on the warnnings related to creation of tasks
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        controller.Value.Refresh();
                        if (controller.Value.Status == ServiceControllerStatus.Stopped)
                        {
                            controller.Value.Start();
                            controller.Value.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(TIME_OUT_IN_MINUTE));
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewMode.ExceptionText = $"Failed starting service {controller.Value.ServiceName}. " +
                            $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                        ViewMode.Logger.SetLogLevel(Logger.LoggingLevel.Error).WriteLog(ViewMode.ExceptionText);
                    }
                }, TaskCreationOptions.RunContinuationsAsynchronously).ContinueWith((x) => { controller.Key.Status = controller.Value.Status; }));
            }

            Task.WaitAll(tasks.ToArray());

            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (this)
                {
                    ViewMode.WindowsServiceCollectionView.Refresh();
                }
            });
        }

    }
}
