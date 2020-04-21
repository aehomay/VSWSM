using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsServiceManager.Helper;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class AttachToServiceCommand : BaseCommand
    {
        public AttachToServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
           ViewMode.ExceptionText = string.Empty;
            var service = Controllers.Keys.FirstOrDefault().ServiceName;
            var svProcess = Utility.GetProcessByServiceName(service);
            var vs = Utility.GetVisualStudioProcesses().ToList();

        }

             
    }
}
