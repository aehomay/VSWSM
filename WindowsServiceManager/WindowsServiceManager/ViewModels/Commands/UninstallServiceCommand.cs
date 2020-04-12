using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class UninstallServiceCommand : BaseCommand
    {
        public UninstallServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            var installer = new ServiceInstaller();
            var Context = new InstallContext("/LogFile=uninstall.log", null);//TODO: support command arguments as well.
            installer.Context = Context;
            
            var confirmation = MessageBox.Show("Are you sure you want to uninstall?", "Uninstall", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (confirmation)
            {
                foreach (var controller in Controllers.Values)
                {
                    try
                    {
                        installer.ServiceName = controller.ServiceName;
                        installer.Uninstall(null);
                    }
                    catch (Exception ex)
                    {
                        ViewMode.ExceptionText = $"Failed unstalling service {controller.ServiceName}. " +
                                $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                    }
                }
                ViewMode.RefreshServiceCommand.Execute(null);
            }
        }
    }
}
