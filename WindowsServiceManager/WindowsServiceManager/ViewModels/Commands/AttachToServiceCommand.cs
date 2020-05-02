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
using WindowsServiceManager.View;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class AttachToServiceCommand : BaseCommand
    {
        public AttachToServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            try
            {
                ViewMode.ExceptionText = string.Empty;
                var service = Controllers.FirstOrDefault().ServiceName;
                var svProcess = Utility.GetProcessByServiceName(service);
                var window = new VisualStudioProcessWindow
                {
                    VisualStudioProcesses = Utility.GetVisualStudioProcesses().Where(vs => !string.IsNullOrEmpty(vs.SolutionName))
                };
                window.ShowDialog();
                var vsProcess = window.SelectedVisualStudioProcess;
                vsProcess?.AttachToProcess(svProcess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
