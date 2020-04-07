using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindowsServiceManager.ViewModel
{
    public class WindowsServiceViewModel : ViewModel
    {
        private string _filterText = string.Empty;
        private string _exceptionText = string.Empty;
        private readonly ObservableCollection<WindowsServiceInfo> _windowsServiceInfos = null;
        private readonly CollectionViewSource _windowsServiceCollection = null;

        public List<WindowsServiceInfo> FilteredItems { get; } = null;
        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                _windowsServiceCollection.View.Refresh();
                OnPropertyChanged(nameof(FilterText));
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
            _windowsServiceInfos = new ObservableCollection<WindowsServiceInfo>();
            _windowsServiceCollection = new CollectionViewSource();
            _windowsServiceCollection.Filter += WindowsServiceCollectionFilter;
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

        public ICollectionView WindowsServiceCollectionView
        {
            get
            {
                return _windowsServiceCollection.View;
            }
        }

        public void Clear()
        {
            _windowsServiceInfos.Clear();
            UpdateInternalCollection();
        }

        public void AddServiceInfo(WindowsServiceInfo serviceInfo)
        {
            _windowsServiceInfos.Add(serviceInfo);
        }

        public void RemoveServiceInfo(WindowsServiceInfo serviceInfo)
        {
            _windowsServiceInfos.Remove(serviceInfo);
        }

        public void UpdateInternalCollection()
        {
            _windowsServiceCollection.Source = _windowsServiceInfos;
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
                            WindowsServiceCollectionView.Refresh();
                        }
                    }
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning).Start();
        }
    }
}
