﻿using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
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
            _ = Task.Factory.StartNew(() =>
            {
                foreach (var controller in ServiceControllers)
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
                    }
                }
            }, new CancellationToken(), TaskCreationOptions.None, TaskScheduler.Default);
        }

        private void TerminateServiceByProcess(string serviceName)
        {
            Utility.GetProcessByServiceName(serviceName)?.Kill();
        }

    }
}
