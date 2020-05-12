using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsServiceManager.Helper;

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
            string[] commandLineOptions = new string[1] { "/LogFile=install.log" };
            IDictionary state = new Hashtable();

            try
            {
                exeFilename = OpenFileDialog();
                Environment.CurrentDirectory = Path.GetDirectoryName(exeFilename);
                if (!string.IsNullOrEmpty(exeFilename))
                {
                    using (AssemblyInstaller installer = new AssemblyInstaller(exeFilename, commandLineOptions))
                    {
                        installer.Install(state);
                        installer.Commit(state);
                        MessageBox.Show($"The {exeFilename} has installed as a Windows service successfully.");
                    }
                }
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
