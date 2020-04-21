using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using DTEProcess = EnvDTE.Process;

namespace WindowsServiceManager.Helper
{
    public class VisualStudioProcess
    {
        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetFocus(IntPtr hWnd);
        public System.Diagnostics.Process Process { get; private set; }

        public _DTE Instance { get; private set; }

        public string SolutionPath 
        {
            get
            {
                return Instance?.Solution.FullName;
            } 
        }

        public string SolutionName
        {
            get
            {
                return Instance?.Solution.FullName.Split('\\').Last();
            }
        }

        public VisualStudioProcess(System.Diagnostics.Process process)
        {
            Process = process;
            if (TryGetVsInstance(out _DTE instance))
                Instance = instance;
        }

        /// <summary>
        /// The method to use to attach visual studio to a specified process.
        /// </summary>
        /// <param name="applicationProcess">
        /// The application process that needs to be debugged.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the application process is null.
        /// </exception>
        public void AttachToProcess(System.Diagnostics.Process applicationProcess)
        {
            if (Instance != null)
            {
                // Find the process you want the VS instance to attach to...
                DTEProcess processToAttachTo =
                    Instance.Debugger.LocalProcesses.Cast<DTEProcess>()
                                        .FirstOrDefault(process => process.ProcessID == applicationProcess.Id);

                // Attach to the process.
                if (processToAttachTo == null)
                    throw new InvalidOperationException(
                        "Visual Studio process cannot find specified application '" + applicationProcess.Id + "'");

                processToAttachTo.Attach();
                ShowWindow((int)Process.MainWindowHandle, 3);
                SetForegroundWindow(Process.MainWindowHandle);
                SetFocus(Process.MainWindowHandle);
            }
        }

        private bool TryGetVsInstance(out _DTE instance)
        {
            IntPtr numFetched = IntPtr.Zero;
            IRunningObjectTable runningTable;
            IEnumMoniker enumMoniker;
            IMoniker[] monikers = new IMoniker[1];

            GetRunningObjectTable(0, out runningTable);
            runningTable.EnumRunning(out enumMoniker);
            enumMoniker.Reset();

            while (enumMoniker.Next(1, monikers, numFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningName;
                monikers[0].GetDisplayName(ctx, null, out runningName);

                object runningVal;
                runningTable.GetObject(monikers[0], out runningVal);

                if (runningVal is _DTE && runningName.StartsWith("!VisualStudio"))
                {
                    int current = int.Parse(runningName.Split(':')[1]);

                    if (current == Process.Id)
                    {
                        instance = runningVal as _DTE;
                        return true;
                    }
                }
            }

            instance = null;
            return false;
        }
    }
}
