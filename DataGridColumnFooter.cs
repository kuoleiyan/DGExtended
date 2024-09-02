using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DGExtend
{
    public class DataGridColumnFooter : ContentControl, INotifyPropertyChanged
    {
        public DataGridColumnFooter()
        {
            ContentChanged += (o, e) => AdjustColumnWidth();
            this.Unloaded += OnUnloaded;
        }
        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ContentChanged -= (o, e) => AdjustColumnWidth();
        }

        public static readonly RoutedEvent ContentChangedEvent = EventManager.RegisterRoutedEvent(
        "ContentChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DataGridColumnFooter));

        public event RoutedEventHandler ContentChanged
        {
            add { AddHandler(ContentChangedEvent, value); }
            remove { RemoveHandler(ContentChangedEvent, value); }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            RaiseEvent(new RoutedEventArgs(ContentChangedEvent));
        }

        public void AdjustColumnWidth()
        {
            if (Column == null) return;

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var content = Content as UIElement;
            double width = 0;
            if (content != null) 
            {
                content.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                var desiredSize = content.DesiredSize;
                width = desiredSize.Width;
            }
            else if (Content is string contentString)
            {
                var textBlock = new TextBlock
                {
                    Text = contentString,
                    FontFamily = FontFamily,
                    FontSize = FontSize,
                    FontStyle = FontStyle,
                    FontWeight = FontWeight,
                    FontStretch = FontStretch
                };
                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                width = textBlock.DesiredSize.Width;
            }
            if (width > Column.ActualWidth)
            {
                Column.Width = width + 4;//margin + padding
            }
        }

        DataGridColumn? _Column;
        public DataGridColumn? Column
        {
            get => _Column;
            set
            {
                if (_Column != value)
                {
                    if (_Column != null)
                    {
                        var descriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.DisplayIndexProperty, typeof(DataGridColumn));
                        descriptor.RemoveValueChanged(_Column, OnDisplayIndexChanged);
                    }

                    _Column = value;

                    if (_Column != null)
                    {
                        var descriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.DisplayIndexProperty, typeof(DataGridColumn));
                        descriptor.AddValueChanged(_Column, OnDisplayIndexChanged);
                    }

                    OnPropertyChanged(nameof(Column));
                    OnPropertyChanged(nameof(DisplayIndex));
                }
            }
        }
        public int? DisplayIndex { get => Column?.DisplayIndex; }

        public static readonly DependencyProperty DisplayColumnIndexProperty = DependencyProperty.Register(
            "DisplayColumnIndex",
            typeof(int?),
            typeof(DataGridColumnFooter),
            new PropertyMetadata(null)
            );
        void OnDisplayIndexChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(DisplayIndex));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int? DisplayColumnIndex
        {
            get => (int?)GetValue(DisplayColumnIndexProperty);
            set => SetValue(DisplayColumnIndexProperty, value);
        }
    }
}
