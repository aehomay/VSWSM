using System.Collections.Generic;
using System.ServiceProcess;
using System.Windows.Controls;
using WindowsServiceManager.ViewModels;

namespace WindowsServiceManager.View
{
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
            var serviceInfo = ((ListViewItem)e.Source).Content as ServiceController;
            var eventWindow = new EventManagerWindow(serviceInfo.ServiceName);
            eventWindow.ShowDialog();
        }
        private void ServiceListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ServiceInfoViewModel.SelectedItems = new List<ServiceController>();
            var services = ((ListView)e.Source).SelectedItems;
            foreach (var service in services)
            {
                ServiceInfoViewModel.SelectedItems.Add((ServiceController)service);
            }            
        }

        #endregion

        #region Private Methods

        #endregion


    }
}