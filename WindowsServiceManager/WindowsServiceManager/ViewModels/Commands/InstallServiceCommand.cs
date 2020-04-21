using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsServiceManager.ViewModels.Commands
{
    public class InstallServiceCommand : BaseCommand
    {
        public InstallServiceCommand(WindowsServiceViewModel vm) : base(vm)
        {
        }

        public override void Execute()
        {
            var exeFilename = string.Empty;
            try
            {
                exeFilename = OpenFileDialog();
                if (!string.IsNullOrEmpty(exeFilename))
                {
                    string[] commandLineOptions = new string[1] { "/LogFile=install.log" };
                    var installer = new AssemblyInstaller(exeFilename, commandLineOptions)
                    {
                        UseNewContext = true
                    };
                    installer.Install(null);
                    installer.Commit(null);
                    MessageBox.Show($"The {exeFilename} has installed as a Windows service successfully.");
                }
            }
            catch (SecurityException ex)
            {
                ViewMode.ExceptionText = $"Failed installing service {exeFilename}. " +
                                $"Exception:{ex.Message}. This could be because of not running the visual studio under admin rights.";
            }
            catch (Exception ex)
            {
                ViewMode.ExceptionText = $"Failed installing service {exeFilename}. " +
                                $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
            }
            ViewMode.RefreshServiceCommand.Execute(null);
        }

        private string OpenFileDialog()
        {
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.exe)|*.exe";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                }
            }
            return filePath;
        }
    }
}
