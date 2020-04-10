using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class StartServiceCommand
    {
        public ServiceController[] Controllers { get;private set; }

        private readonly WindowsServiceViewModel vM;
        public StartServiceCommand(WindowsServiceViewModel viewModel)
        {
            vM = viewModel;
        }

        public void StartServices(ServiceController[] controllers)
        {
            Controllers = controllers;
            vM.ExceptionText = string.Empty;
            foreach (var controller in Controllers)
            {
                new Task(() =>
                {
                    try
                    {
                        controller.Refresh();
                        if (controller.Status == ServiceControllerStatus.Stopped)
                        {
                            controller.Start();
                        }
                    }
                    catch (Exception ex)
                    {
                        vM.ExceptionText = $"Failed starting service {controller.ServiceName}. " +
                            $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                        vM.Logger.SetLogLevel(Logger.LoggingLevel.Error).WriteLog(vM.ExceptionText);
                    }
                }, TaskCreationOptions.RunContinuationsAsynchronously).Start();
            }
        }
    }
}
