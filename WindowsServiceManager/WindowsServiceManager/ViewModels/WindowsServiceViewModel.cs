using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using WindowsServiceManager.ViewModels.Commands;

namespace WindowsServiceManager.ViewModels
{
    public class WindowsServiceViewModel : ViewModelBase
    {
        const int TIME_OUT_IN_MINUTE = 1;

        private string _filterText = string.Empty;
        private string _exceptionText = string.Empty;

        private readonly ObservableCollection<WindowsServiceInfo> windowsServiceInfos = null;
        private readonly CollectionViewSource windowsServiceCollection = null;

        private RelayCommand<object> mouseRightButtonDownCommand;
        public RelayCommand<object> MouseRightButtonDownCommand
        {
            get
            {
                if (mouseRightButtonDownCommand == null) return mouseRightButtonDownCommand = new RelayCommand<object>(param => ExecuteMouseMove((MouseEventArgs)param));
                return mouseRightButtonDownCommand;
            }
            set { mouseRightButtonDownCommand = value; }
        }

        private void ExecuteMouseMove(MouseEventArgs e)
        {
            Console.WriteLine("Mouse Move : " + e.GetPosition((IInputElement)e.Source));
        }

        public RelayCommand<ServiceController[]> StartServiceCommand
        {
            get
            {
                var command = new StartServiceCommand(this);
                return new RelayCommand<ServiceController[]>(x=> command.StartServices(x));
            }
        }

        public List<object> SelectedItems { get; set; }

        public List<WindowsServiceInfo> FilteredItems { get; } = null;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                lock (windowsServiceCollection)
                {
                    windowsServiceCollection.View.Refresh();
                }
                OnPropertyChanged(nameof(FilterText));
            }
        }
        public ICollectionView WindowsServiceCollectionView
        {
            get
            {
                return windowsServiceCollection.View;
            }
        }

        public string ExceptionText
        {
            get => _exceptionText;
            set
            {
                _exceptionText = value;
                OnPropertyChanged(nameof(ExceptionText));
            }
        }

        public WindowsServiceViewModel()
        {
            FilteredItems = new List<WindowsServiceInfo>();
            windowsServiceInfos = new ObservableCollection<WindowsServiceInfo>();
            windowsServiceCollection = new CollectionViewSource();
            windowsServiceCollection.Filter += WindowsServiceCollectionFilter;

            BindWindowsServices();
            Refresh();
        }

        private void WindowsServiceCollectionFilter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                FilteredItems.Clear();
                e.Accepted = true;
                return;
            }
            var wsInfo = e.Item as WindowsServiceInfo;
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

        public void RemoveServiceInfo(WindowsServiceInfo serviceInfo)
        {
            windowsServiceInfos.Remove(serviceInfo);
        }

        public void UpdateInternalCollection()
        {
            windowsServiceCollection.Source = windowsServiceInfos;
            OnPropertyChanged(nameof(WindowsServiceCollectionView));
        }

        private void Refresh()
        {
            new Task(() =>
            {
                while (true)
                {
                    var services = FilteredItems;
                    foreach (var service in services)
                    {
                        var controller = ServiceController.GetServices().FirstOrDefault(c => c.ServiceName.ToUpper().Equals(service.ServiceName.ToUpper()));
                        if (controller != null)
                        {
                            service.Status = controller.Status;
                        }
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        lock (this)
                        {
                            windowsServiceCollection.View.Refresh();
                        }
                    });
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning).Start();
        }

        private void BindWindowsServices()
        {
            // Display the list of services currently running on this computer.
            try
            {
                windowsServiceInfos.Clear();
                var controllers = ServiceController.GetServices();
                foreach (ServiceController controller in controllers)
                    windowsServiceInfos.Add(new WindowsServiceInfo
                    {
                        ServiceName = controller.ServiceName,
                        ServiceType = controller.ServiceType,
                        Status = controller.Status,
                        DisplayName = controller.DisplayName,
                        MachineName = controller.MachineName
                    });
                UpdateInternalCollection();
            }
            catch (System.Exception ex)
            {
                Logger.SetLogLevel(Logger.LoggingLevel.Error).WriteLog(ex.Message);
            }
        }

        public void HandleStartServices(ServiceController[] controllers)
        {
            StartServiceCommand.Execute(controllers);
        }

        public void HandleStopServices(ServiceController[] controllers)
        {
            ExceptionText = string.Empty;
            var sorted = DependencyOrder(controllers).ToList();
            foreach (var controller in sorted)
            {
                if (controller.Status == ServiceControllerStatus.Running)
                {
                    try
                    {
                        if (controller.CanStop)
                        {
                            controller.Stop();
                            controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(TIME_OUT_IN_MINUTE));
                        }
                    }
                    catch (System.ServiceProcess.TimeoutException)
                    {
                        ExceptionText = $"Time-out of {TimeSpan.FromMinutes(TIME_OUT_IN_MINUTE)} minutes has arrived for the Service {controller.ServiceName} " +
                            $"with service stop request.";
                        Logger.SetLogLevel(Logger.LoggingLevel.Warning).WriteLog(ExceptionText);
                    }
                    catch (Exception ex)
                    {
                        ExceptionText = $"Exception happed during the service stop request. " +
                            $"Exception: {ex.Message} InnerException: {ex.InnerException}";
                        Logger.SetLogLevel(Logger.LoggingLevel.Critical).WriteLog(ExceptionText);
                    }
                }
            }
        }

        private IEnumerable<ServiceController> DependencyOrder(ServiceController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.DependentServices.Length > 0)
                    DependencyOrder(controller.DependentServices);
                yield return controller;
            }
        }
    }
}
