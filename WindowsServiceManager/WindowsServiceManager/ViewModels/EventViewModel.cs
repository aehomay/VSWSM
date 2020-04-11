﻿using GalaSoft.MvvmLight;
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
        private readonly CollectionViewSource _eventCollectionView = null;
        private string _filterText = string.Empty;
        private string _exceptionText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(() => FilterText, ref _filterText, value);
                _eventCollectionView.View.Refresh();
            }
        }

        public string ExceptionText
        {
            get => _exceptionText;
            set
            {
                Set(() => ExceptionText, ref _exceptionText, value);
            }
        }

        public ICollectionView EventCollectionView
        {
            get
            {
                return _eventCollectionView.View;
            }
        }

        public EventViewModel()
        {
            _eventCollectionView = new CollectionViewSource();
            _eventCollectionView.Filter += EventViewSourceFilter;
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
            _eventCollectionView.Source = eventLog.Entries;
            RaisePropertyChanged(nameof(EventCollectionView));
        }
    }
}
