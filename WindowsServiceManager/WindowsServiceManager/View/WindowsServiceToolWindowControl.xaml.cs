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
        private void MenuItemKill_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemStop_Click(object sender, RoutedEventArgs e)
        {
            var services = ResolveSelectedServices(sender);
            var controllers = ToServiceControllers(services);
            ServiceInfoViewModel.HandleStopServices(controllers.ToArray());
        }
        private void MenuItemStart_Click(object sender, RoutedEventArgs e)
        {
            var services = ResolveSelectedServices(sender);
            var controllers = ToServiceControllers(services);
            ServiceInfoViewModel.HandleStartServices(controllers.ToArray());
        }
        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var serviceInfo = ((ListViewItem)e.Source).Content as WindowsServiceInfo;
            var eventWindow = new EventManagerWindow(serviceInfo.ServiceName);
            eventWindow.ShowDialog();
        }

        #endregion

        #region Private Methods
     
        private IList ResolveSelectedServices(object sender)
        {
            var menuItem = (MenuItem)sender;
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (ListView)contextMenu.PlacementTarget;
            return item.SelectedItems;
        }

        private List<ServiceController> ToServiceControllers(IList services)
        {
            var controllers = ServiceController.GetServices();
            var result = new List<ServiceController>();
            foreach (var service in services)
            {
                var controller = controllers.FirstOrDefault(s => s.ServiceName.ToUpper().Equals(((WindowsServiceInfo)service).ServiceName.ToUpper()));
                if (controller != null)
                    result.Add(controller);
                else
                {
                    ServiceInfoViewModel.RemoveServiceInfo((WindowsServiceInfo)service);
                    ServiceInfoViewModel.UpdateInternalCollection();
                }
            }
            return result;
        }


        #endregion
    }
}