using System;
using System.Collections.Generic;
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
using WindowsServiceManager.Helper;

namespace WindowsServiceManager.View
{
    /// <summary>
    /// Interaction logic for VisualStudioProcessWindow.xaml
    /// </summary>
    public partial class VisualStudioProcessWindow : Window
    {
        public IEnumerable<VisualStudioProcess> VisualStudioProcesses { get; set; }

        public VisualStudioProcess SelectedVisualStudioProcess { get; private set; }
        public VisualStudioProcessWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = e.Source as ListViewItem;
            SelectedVisualStudioProcess = item.Content as VisualStudioProcess;
            this.Close();
        }
    }
}
