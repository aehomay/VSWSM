using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindowsServiceManager.ViewModels
{
    public enum LogType
    {
        Application,
        Security,
        Setup,
        System
    }

    public class EventViewModel : ViewModelBase
    {
        private string watermarkText = null;
        private readonly CollectionViewSource eventCollectionView = null;
        private string filterText = string.Empty;
        private string exceptionText = string.Empty;
        public string FilterText
        {
            get => filterText;
            set
            {
                Set(() => FilterText, ref filterText, value);
                eventCollectionView.View.Refresh();
            }
        }

        public string ExceptionText
        {
            get => exceptionText;
            set
            {
                Set(() => ExceptionText, ref exceptionText, value);
            }
        }


        /// <summary>
        /// Gets and sets the text of the watermark
        /// </summary>
        public string WatermarkText
        {
            get
            {
                if (watermarkText == null)
                { watermarkText = string.Empty; }

                return watermarkText;
            }
            set { Set(() => WatermarkText, ref watermarkText, value); }
        }

        public ICollectionView EventCollectionView
        {
            get
            {
                return eventCollectionView.View;
            }
        }

        public EventViewModel()
        {
            WatermarkText = "Enter service name to search!";
            eventCollectionView = new CollectionViewSource();
            eventCollectionView.Filter += EventViewSourceFilter;
            LoadEvents(LogType.Application);
        }

        private void EventViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                e.Accepted = true;
                return;
            }
            var log = e.Item as EventLogEntry;
            e.Accepted = (log.Source.ToUpper().Contains(FilterText.ToUpper())) ? true : false;
        }

        public void LoadEvents(LogType type)
        {
            var eventLog = new EventLog(Enum.GetName(typeof(LogType), type));
            eventCollectionView.Source = eventLog.Entries;
            RaisePropertyChanged(nameof(EventCollectionView));
        }
    }
}
