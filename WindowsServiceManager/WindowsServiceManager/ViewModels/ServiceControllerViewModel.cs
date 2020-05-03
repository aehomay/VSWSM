using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsServiceManager.Helper;

namespace WindowsServiceManager.ViewModel
{
    /// <summary>
    /// This class represent windows service information.
    /// </summary>
    public class ServiceControllerViewModel : ViewModelBase
    {
        bool _visiable = false;
        ServiceControllerStatus _status;
        PauseTokenSource _pauseToken = new PauseTokenSource();
        public ServiceControllerViewModel(ServiceController controller)
        {
            Controller = controller ?? throw new ArgumentNullException(paramName: nameof(controller));
            _ = UpdateServiceStatusAsync(_pauseToken.Token, CancellationToken.None);
        }

        public bool Visiable
        {
            get => _visiable;
            set
            {
                if (value)
                    _ = _pauseToken.ResumeAsync();
                else
                    _ = _pauseToken.PauseAsync();
                _visiable = value;
            }
        }

        /// <summary>
        /// Gets or sets the name that identifies the service that this instance references.
        /// </summary>
        public string ServiceName => Controller.ServiceName;

        /// <summary>
        /// Gets or sets the name of the computer on which this service resides.
        /// </summary>
        public string MachineName => Controller.MachineName;

        /// <summary>
        /// Gets or sets a friendly name for the service.
        /// </summary>
        public string DisplayName => Controller.DisplayName;

        /// <summary>
        /// Gets the status of the service that is referenced by this instance.
        /// </summary>
        public ServiceControllerStatus Status
        {
            get => _status; set { Set(() => Status, ref _status, value); }
        }

        public ServiceController Controller { get; } = null;

        /// <summary>
        /// Gets the type of service that this object references.
        /// </summary>
        public ServiceType ServiceType => Controller.ServiceType;

        public async Task UpdateServiceStatusAsync(PauseToken pause, CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                await pause.PauseIfRequestedAsync();
                Controller.Refresh();
                Status = Controller.Status;
                await Task.Delay(500);
            }
        }

    }
}
