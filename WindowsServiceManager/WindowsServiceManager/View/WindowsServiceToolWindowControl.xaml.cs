namespace WindowsServiceManager.View
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using WindowsServiceManager.Model;
    using WindowsServiceManager.View;
    using WindowsServiceManager.ViewModels;

    /// <summary>
    /// Interaction logic for WindowsServiceToolWindowControl.
    /// </summary>
    public partial class WindowsServiceToolWindowControl : UserControl
    {
        readonly WindowsServiceViewModel ServiceInfoViewModel = new WindowsServiceViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceToolWindowControl"/> class.
        /// </summary>
        public WindowsServiceToolWindowControl()
        {
            this.InitializeComponent();
            this.DataContext = ServiceInfoViewModel;
        }

        #region Events
        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var serviceInfo = ((ListViewItem)e.Source).Content as WindowsServiceInfo;
            var eventWindow = new EventManagerWindow(serviceInfo.ServiceName);
            eventWindow.ShowDialog();
        }
        private void ServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var services = ((ListView)e.Source).SelectedItems;
            ServiceInfoViewModel.SelectedItems = ToServiceControllers(services);
        }
        #endregion

        #region Private Methods

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

        #endregion


    }
}