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
using WindowsServiceManager.ViewModels;

namespace WindowsServiceManager.View
{
    /// <summary>
    /// Interaction logic for EventManagerWindow.xaml
    /// </summary>
    public partial class EventManagerWindow : Window
    {
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
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var log = ((ListViewItem)e.Source).Content as EventLogEntry;
            EventViewModel.ExceptionText = log.Message;
        }
    }
}
