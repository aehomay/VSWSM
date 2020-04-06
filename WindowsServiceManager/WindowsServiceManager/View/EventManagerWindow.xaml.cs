using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsServiceManager.ViewModel;

namespace WindowsServiceManager
{
    /// <summary>
    /// Interaction logic for EventManagerWindow.xaml
    /// </summary>
    public partial class EventManagerWindow : Window
    {
        const string LOG_FILE_NAME = "WindowsServiceManager.log";
        private Logger logger = null;
        readonly EventViewModel EventViewModel = new EventViewModel();
        public EventManagerWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public EventManagerWindow(string logName)
        {
            InitializeComponent();
            Initialize();
            EventViewModel.FilterText = logName;
        }

        private void Initialize()
        {
            this.DataContext = EventViewModel;
            var path = Environment.CurrentDirectory;
            logger = new Logger(path, LOG_FILE_NAME);
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var log = ((ListViewItem)e.Source).Content as EventLogEntry;
            EventViewModel.ExceptionText = log.Message;
        }
    }
}
