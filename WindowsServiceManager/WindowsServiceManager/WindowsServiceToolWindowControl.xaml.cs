namespace WindowsServiceManager
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
    using WindowsServiceManager.ViewModel;

    /// <summary>
    /// Interaction logic for WindowsServiceToolWindowControl.
    /// </summary>
    public partial class WindowsServiceToolWindowControl : UserControl
    {
        const string LOG_FILE_NAME = "WindowsServiceManager.log";
        const int TIME_OUT_IN_MINUTE = 1;
        private Logger logger = null;
        readonly WindowsServiceInfoViewModel ServiceInfoViewModel = new WindowsServiceInfoViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsServiceToolWindowControl"/> class.
        /// </summary>
        public WindowsServiceToolWindowControl()
        {
            this.InitializeComponent();
            Initialize();
        }

        #region Events
        private void MenuItemKill_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemStop_Click(object sender, RoutedEventArgs e)
        {
            var services = ResolveSelectedServices(sender);
            var controllers = ToServiceControllers(services);
            HandleStopServices(controllers.ToArray());
        }
        private void MenuItemStart_Click(object sender, RoutedEventArgs e)
        {
            var services = ResolveSelectedServices(sender);
            var controllers = ToServiceControllers(services);
            HandleStartServices(controllers.ToArray());
        }
        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var serviceInfo = ((ListViewItem)e.Source).Content as WindowsServiceInfo;
            var eventWindow = new EventManagerWindow(serviceInfo.ServiceName);
            eventWindow.ShowDialog();
        }

        #endregion

        #region Private Methods
        private void Initialize()
        {
            var path = Environment.CurrentDirectory;
            logger = new Logger(path, LOG_FILE_NAME);
            this.DataContext = ServiceInfoViewModel;
            BindWindowsServices();
            Refresh();
        }

        private void BindWindowsServices()
        {
            // Display the list of services currently running on this computer.
            try
            {
                ServiceInfoViewModel.Clear();
                var controllers = ServiceController.GetServices();
                foreach (ServiceController controller in controllers)
                    ServiceInfoViewModel.AddServiceInfo(new WindowsServiceInfo
                    {
                        ServiceName = controller.ServiceName,
                        ServiceType = controller.ServiceType,
                        Status = controller.Status,
                        DisplayName = controller.DisplayName,
                        MachineName = controller.MachineName
                    });
                ServiceInfoViewModel.UpdateInternalCollection();
            }
            catch (System.Exception ex)
            {
                logger.SetLogLevel(Logger.LoggingLevel.Error).WriteLog(ex.Message);
            }
        }
        
        private void Refresh()
        {
            new Task(() =>
            {
                while (true)
                {
                    var services = ServiceInfoViewModel.FilteredItems;
                    Dispatcher.Invoke(() =>
                    {
                        foreach (var service in services)
                        {
                            var controller = ServiceController.GetServices().FirstOrDefault(c => c.ServiceName.ToUpper().Equals(service.ServiceName.ToUpper()));
                            if (controller != null)
                            {
                                service.Status = controller.Status;
                                ServiceInfoViewModel.WindowsServiceCollectionView.Refresh();
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning).Start();
        }

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

        private void HandleStartServices(ServiceController[] serviceController)
        {
            ServiceInfoViewModel.ExceptionText = string.Empty;
            foreach (var controller in serviceController)
            {
                new Task(() =>
                  {
                      try
                      {
                          controller.Refresh();
                          if (controller.Status == ServiceControllerStatus.Stopped)
                          {
                              controller.Start();
                          }
                      }
                      catch (Exception ex)
                      {
                          ServiceInfoViewModel.ExceptionText = $"Failed starting service {controller.ServiceName}. " +
                              $"Exception:{ex.Message}. InnerException:{ex.InnerException}";
                          logger.SetLogLevel(Logger.LoggingLevel.Error).WriteLog(ServiceInfoViewModel.ExceptionText);
                      }
                  }).Start();
            }
        }

        private void HandleStopServices(ServiceController[] controllers)
        {
            ServiceInfoViewModel.ExceptionText = string.Empty;
            var sorted = DependencySorter(controllers).ToList();
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
                        ServiceInfoViewModel.ExceptionText = $"Time-out of {TimeSpan.FromMinutes(TIME_OUT_IN_MINUTE)} minutes has arrived for the Service {controller.ServiceName} " +
                            $"with service stop request.";
                        logger.SetLogLevel(Logger.LoggingLevel.Warning).WriteLog(ServiceInfoViewModel.ExceptionText);
                    }
                    catch (Exception ex)
                    {
                        ServiceInfoViewModel.ExceptionText = $"Exception happed during the service stop request. " +
                            $"Exception: {ex.Message} InnerException: {ex.InnerException}";
                        logger.SetLogLevel(Logger.LoggingLevel.Critical).WriteLog(ServiceInfoViewModel.ExceptionText);
                    }
                }
            }
        }

        private IEnumerable<ServiceController> DependencySorter(ServiceController[] controllers)
        {
            foreach (var controller in controllers)
            {
                if (controller.DependentServices.Length > 0)
                    DependencySorter(controller.DependentServices);
                yield return controller;
            }
        }

        #endregion
    }
}