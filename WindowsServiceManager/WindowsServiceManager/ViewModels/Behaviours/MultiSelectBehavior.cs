using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace WindowsServiceManager.ViewModels.Behaviours
{
    public class MultiSelectBehavior : Behavior<ListView>
    {
        #region Private Fields
        private bool selectionChangedInProgress;
        #endregion

        #region Public Members
        public ObservableCollection<object> SelectedItems
        {
            get { return (ObservableCollection<object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            nameof(SelectedItems),
            typeof(ObservableCollection<object>),
            typeof(MultiSelectBehavior),
            new PropertyMetadata(new ObservableCollection<object>(), PropertyChangedCallback));
        #endregion

        #region Protected Members
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }
        #endregion

        #region Private Members
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectionChangedInProgress) return;
            selectionChangedInProgress = true;
            foreach (var item in e.RemovedItems)
            {
                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (!SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                }
            }
            selectionChangedInProgress = false;
        }

        private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler handler = (s, e) => SelectedItemsChanged(sender, e);
            if (args.OldValue is ObservableCollection<object>)
            {
                (args.OldValue as ObservableCollection<object>).CollectionChanged -= handler;
            }

            if (args.NewValue is ObservableCollection<object>)
            {
                (args.NewValue as ObservableCollection<object>).CollectionChanged += handler;
            }
        }

        private static void SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is MultiSelectBehavior)
            {
                var listViewBase = (sender as MultiSelectBehavior).AssociatedObject;

                var listSelectedItems = listViewBase.SelectedItems;
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Remove(item);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (!listSelectedItems.Contains(item))
                        {
                            listSelectedItems.Add(item);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
