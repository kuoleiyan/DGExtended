using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DGExtend
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class DataGridEx : DataGrid
    {
        public DataGridEx()
        {
            InitializeComponent();
            Loaded += DataGrid_Loaded;
            Footers.Clear();
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGrid));
            dpd.AddValueChanged(this, OnItemsSourceChanged!);
        }
        public event EventHandler? ItemsSourceChanged;
        private INotifyCollectionChanged? oldCollection;
        private void OnItemsSourceChanged(object sender, EventArgs e)
        {
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= OnCollectionChanged!;
            }

            if (ItemsSource is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnCollectionChanged!;
                oldCollection = newCollection;
            }
            else
            {
                oldCollection = null;
            }

            ItemsSourceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemsSourceChanged?.Invoke(this, EventArgs.Empty);
        }

        public static readonly DependencyProperty FootersProperty = DependencyProperty.Register(
            "Footers",
            typeof(ObservableCollection<DataGridColumnFooter>),
            typeof(DataGridEx),
            new PropertyMetadata(new ObservableCollection<DataGridColumnFooter>())
            );

        public ObservableCollection<DataGridColumnFooter> Footers
        {
            get => (ObservableCollection<DataGridColumnFooter>)GetValue(FootersProperty);
        }

        internal DataGridColumnFootersPresenter? FootersPresenter;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            FootersPresenter?.SetFooterCollection(Footers);
        }

        private static T? GetVisualChild<T>(Visual parent) where T : Visual
        {
            T? child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }

}
