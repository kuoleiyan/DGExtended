using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DGExtend
{
    public class DataGridColumnFootersPresenter : ItemsControl
    {
        DataGridEx? _parentDataGrid = null;
        internal DataGridEx? ParentDataGrid
        {
            get
            {
                if (_parentDataGrid == null)
                {
                    _parentDataGrid = FindParent<DataGridEx>(this);
                }

                return _parentDataGrid;
            }
        }
        ScrollViewer? _scrollViewer = null;
        internal ScrollViewer? ScrollViewer
        {
            get
            {
                if (_scrollViewer == null)
                {
                    _scrollViewer = FindParent<ScrollViewer>(this);
                }

                return _scrollViewer;
            }
        }
        public ObservableCollection<DataGridColumnFooter> FooterCollection { get; set; }
        CollectionViewSource CVS = new CollectionViewSource();
        public ObservableCollection<DataGridColumn>? Columns;

        public DataGridColumnFootersPresenter()
        {
            FooterCollection = new();
            this.Unloaded += OnUnloaded;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement? child = (VisualTreeHelper.GetChildrenCount(this) > 0) ? VisualTreeHelper.GetChild(this, 0) as UIElement : null;

            if (child != null)
            {
                Rect childRect = new Rect(finalSize);
                DataGrid? dataGrid = ParentDataGrid;
                if (dataGrid != null && ScrollViewer != null)
                {
                    childRect.Width = Math.Max(finalSize.Width, ScrollViewer.ViewportWidth - dataGrid.RowHeaderActualWidth);
                }

                child.Arrange(childRect);
            }
            return finalSize;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            DataGridEx? grid = ParentDataGrid;
            if (grid != null)
            {
                grid.FootersPresenter = this;
                Columns = grid.Columns;
                SetFooterCollection(grid.Footers);
                CVS.Source = FooterCollection;
                CVS.SortDescriptions.Add(new SortDescription("DisplayIndex", ListSortDirection.Ascending));
                ItemsSource = CVS.View;
            }
            if (ScrollViewer != null)
                ScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
        }
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (ScrollViewer != null)
                this.RenderTransform = new TranslateTransform(-ScrollViewer.HorizontalOffset, 0);
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (ScrollViewer != null)
                ScrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
        }


        public void SetFooterCollection(IEnumerable<DataGridColumnFooter> footers)
        {
            FooterCollection.Clear();
            if (Columns == null) return;
            foreach (var f in footers)//when not set DisplayColumn auto fill it.
            {
                if (f.DisplayColumnIndex == null)
                {
                    Span<int> indexs = new Span<int>(footers.Where(x => x.DisplayColumnIndex.HasValue).Select(x => x.DisplayColumnIndex!.Value).ToArray());

                    for (int i = 0; i < Columns.Count; i++)
                    {
                        if (!indexs.Contains(i))
                        {
                            f.DisplayColumnIndex = i;
                            break;
                        }
                    }
                }
            }
            for (var i = 0; i < Columns.Count; i++) //gen FooterCollection item
            {
                Binding bindingActualWidth = new Binding("ActualWidth");
                bindingActualWidth.Source = Columns[i];
                Binding bindingVisibility = new Binding("Visibility");
                bindingVisibility.Source = Columns[i];
                var footer = footers.LastOrDefault(f => f.DisplayColumnIndex == i);
                if (footer == null)
                {
                    var f = new DataGridColumnFooter() { Column = Columns[i] };
                    f.SetBinding(WidthProperty, bindingActualWidth);
                    f.SetBinding(VisibilityProperty, bindingVisibility);
                    FooterCollection.Add(f);
                }
                else
                {
                    footer.DataContext = DataContext;
                    footer.Column = Columns[i];
                    footer.AdjustColumnWidth();
                    footer.SetBinding(WidthProperty, bindingActualWidth);
                    footer.SetBinding(VisibilityProperty, bindingVisibility);
                    FooterCollection.Add(footer);
                }
            }
            foreach (var f in FooterCollection) //refresh view when DisplayIndex Changed
            {
                f.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "DisplayIndex")
                    {
                        CVS.View.Refresh();
                    }
                };
            }
        }

        internal static T? FindParent<T>(FrameworkElement element) where T : FrameworkElement
        {
            FrameworkElement? parent = element.TemplatedParent as FrameworkElement;

            while (parent != null)
            {
                T? correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = parent.TemplatedParent as FrameworkElement;
            }

            return null;
        }
    }
}
