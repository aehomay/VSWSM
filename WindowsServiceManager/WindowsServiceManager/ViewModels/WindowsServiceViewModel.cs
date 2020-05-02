using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WindowsServiceManager.ViewModels.Commands;

namespace WindowsServiceManager.ViewModels
{
    public class WindowsServiceViewModel : ViewModelBase
    {
        private string _WatermarkText = null;
        private string _filterText = string.Empty;
        private string _exceptionText = string.Empty;
        private ObservableCollection<ServiceController> serviceControllers = null;
        private readonly CollectionViewSource serviceControllerView = null;

        #region Dead Code
        private RelayCommand<object> mouseRightButtonDownCommand;
        public RelayCommand<object> MouseRightButtonDownCommand
        {
            get
            {
                if (mouseRightButtonDownCommand == null) return mouseRightButtonDownCommand =
                        new RelayCommand<object>(param => OnMouseRightButtonDown((MouseEventArgs)param), (x) => { return true; });
                return mouseRightButtonDownCommand;
            }
            set { mouseRightButtonDownCommand = value; }
        }
        private void OnMouseRightButtonDown(MouseEventArgs e)
        {
            var services = ((ListView)e.Source).SelectedItems;
            SelectedItems = (List<ServiceController>)services;
        }
        #endregion

        public RelayCommand<object> StartServiceCommand
        {
            get
            {
                var command = new StartServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) =>
                {
                    return command.Controllers != null && command.Controllers.Count > 0 &&
               command.Controllers.Any(c => c.Status == ServiceControllerStatus.Stopped);
                });
            }
        }

        public RelayCommand<object> StopServiceCommand
        {
            get
            {
                var command = new StopServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) =>
                {
                    return command.Controllers != null && command.Controllers.Count > 0 &&
               command.Controllers.Any(c => c.Status == ServiceControllerStatus.Running);
                });
            }
        }

        public RelayCommand<object> KillServiceCommand
        {
            get
            {
                var command = new TerminateServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) =>
                {
                    return command.Controllers != null && command.Controllers.Count > 0 &&
               command.Controllers.Any(c => c.Status != ServiceControllerStatus.Stopped);
                });
            }
        }

        public RelayCommand<object> UninstallServiceCommand
        {
            get
            {
                var command = new UninstallServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) =>
                {
                    return command.Controllers != null && command.Controllers.Count > 0 &&
               command.Controllers.Any(c => c.Status == ServiceControllerStatus.Running || c.Status == ServiceControllerStatus.Stopped);
                });
            }
        }

        public RelayCommand<object> AttachToServiceCommand
        {
            get
            {
                var command = new AttachToServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) =>
                {
                    return command.Controllers != null && command.Controllers.Count > 0 &&
               command.Controllers.Any(c => c.Status == ServiceControllerStatus.Running);
                });
            }
        }

        public RelayCommand<object> InstallServiceCommand
        {
            get
            {
                var command = new InstallServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute());
            }
        }

        public RelayCommand<object> RefreshServiceCommand
        {
            get
            {
                return new RelayCommand<object>(x => BindWindowsServices());
            }
        }

        public List<ServiceController> SelectedItems { get; set; }

        public List<ServiceController> FilteredItems { get; } = null;

        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(() => FilterText, ref _filterText, value);
                lock (serviceControllerView)
                {
                    serviceControllerView.View.Refresh();
                }
            }
        }

        public ICollectionView WindowsServiceCollectionView
        {
            get
            {
                return serviceControllerView.View;
            }
        }

        /// <summary>
        /// Gets and sets the text of the watermark
        /// </summary>
        public string WatermarkText
        {
            get
            {
                if (_WatermarkText == null)
                { _WatermarkText = string.Empty; }

                return _WatermarkText;
            }
            set { Set(() => WatermarkText, ref _WatermarkText, value); }
        }

        public string ExceptionText
        {
            get => _exceptionText;
            set
            {
                Set(() => ExceptionText, ref _exceptionText, value);
            }
        }

        public WindowsServiceViewModel()
        {
            WatermarkText = "Enter service name to search!";
            FilteredItems = new List<ServiceController>();
            serviceControllerView = new CollectionViewSource();
            serviceControllerView.Filter += WindowsServiceCollectionFilter;
            BindWindowsServices();
            RefreshSrviceStatus();
        }

        private void RefreshSrviceStatus()
        {
            _ = Task.Factory.StartNew(() =>
             {
                 while (true)
                 {
                     Application.Current.Dispatcher.Invoke(() =>
                     {
                         serviceControllerView.View.Refresh();
                     });
                     Thread.Sleep(500);
                 }
             }, new CancellationToken(), TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void WindowsServiceCollectionFilter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                FilteredItems.Clear();
                e.Accepted = true;
                return;
            }
            var wsInfo = e.Item as ServiceController;
            e.Accepted = (wsInfo.ServiceName.ToUpper().Contains(FilterText.ToUpper())) ? true : false;
            if (e.Accepted)
            {
                if (!FilteredItems.Exists(i => i.ServiceName.ToUpper().Equals(wsInfo.ServiceName.ToUpper())))
                    FilteredItems.Add(wsInfo);
            }
            else
            {
                if (FilteredItems.Exists(i => i.ServiceName.ToUpper().Equals(wsInfo.ServiceName.ToUpper())))
                    FilteredItems.Remove(wsInfo);
            }
        }

        private void BindWindowsServices()
        {
            // Display the list of services currently running on this computer.
            try
            {
                var controllers = ServiceController.GetServices();
                serviceControllers = new ObservableCollection<ServiceController>(controllers);
                serviceControllerView.Source = serviceControllers;
                RaisePropertyChanged(nameof(WindowsServiceCollectionView));
            }
            catch (Exception ex)
            {
                ExceptionText = $"An exception happened while binding services. Exception:{ex.Message}";
            }
        }
    }
}
