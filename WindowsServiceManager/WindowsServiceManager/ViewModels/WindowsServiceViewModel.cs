using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WindowsServiceManager.Model;
using WindowsServiceManager.ViewModels.Commands;

namespace WindowsServiceManager.ViewModels
{
    public class WindowsServiceViewModel : ViewModelBase
    {
        private string _filterText = string.Empty;
        private string _exceptionText = string.Empty;
        private readonly ObservableCollection<WindowsServiceInfo> windowsServiceInfos = null;
        private readonly CollectionViewSource windowsServiceCollection = null;

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
            SelectedItems = ToServiceControllers(services);
        }
        #endregion

        public RelayCommand<object> StartServiceCommand
        {
            get
            {
                var command = new StartServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) => { return command.Controllers != null && command.Controllers.Count > 0 &&
                    command.Controllers.Any(c => c.Key.Status != ServiceControllerStatus.Running); });
            }
        }

        public RelayCommand<object> StopServiceCommand
        {
            get
            {
                var command = new StopServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) => { return command.Controllers != null && command.Controllers.Count > 0 &&
                    command.Controllers.Any(c => c.Key.Status != ServiceControllerStatus.Stopped); });
            }
        }

        public RelayCommand<object> KillServiceCommand
        {
            get
            {
                var command = new StopServiceCommand(this);
                return new RelayCommand<object>(x => command.Execute(),
                (x) => { return command.Controllers != null && command.Controllers.Count > 0 && 
                    command.Controllers.Any(c => c.Key.Status != ServiceControllerStatus.Stopped); });
            }
        }

        public Dictionary<WindowsServiceInfo, ServiceController> SelectedItems { get; set; }

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

        private Dictionary<WindowsServiceInfo, ServiceController> ToServiceControllers(IList services)
        {
            var controllers = ServiceController.GetServices();
            var result = new Dictionary<WindowsServiceInfo, ServiceController>();
            foreach (WindowsServiceInfo service in services)
            {
                var controller = controllers.FirstOrDefault(s => s.ServiceName.ToUpper().Equals(service.ServiceName.ToUpper()));
                if (controller != null)
                    result.Add(service, controller);
            }
            return result;
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
                windowsServiceCollection.Source = windowsServiceInfos;
                OnPropertyChanged(nameof(WindowsServiceCollectionView));
            }
            catch (System.Exception ex)
            {
                Logger.SetLogLevel(Logger.LoggingLevel.Error).WriteLog(ex.Message);
            }
        }

        
    }
}
