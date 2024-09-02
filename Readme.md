# How to use :

**Add xmlns**

```
xmlns:dg="clr-namespace:DGExtend;assembly=DGExtended"
```

**Define DataGridColumnFooter in dg:DataGridEx.Footers**

```
<dg:DataGridEx AutoGenerateColumns="False" ItemsSource="{Binding testdata}">
    <dg:DataGridEx.Footers>
        <dg:DataGridColumnFooter DisplayColumnIndex="2" Content="test" />
        <dg:DataGridColumnFooter Content="test" />
        <dg:DataGridColumnFooter>
            <Border
                Height="30"
                Margin="-2"
                Background="Gold"
                BorderBrush="Blue"
                BorderThickness="1">
                <TextBlock Text="{Binding myfooter}" />
            </Border>
        </dg:DataGridColumnFooter>
    </dg:DataGridEx.Footers>
    <dg:DataGridEx.Columns>
        <DataGridTextColumn Binding="{Binding [0]}" Header="1" />
        <DataGridTextColumn Binding="{Binding [1]}" Header="2" />
        <DataGridTextColumn Binding="{Binding [2]}" Header="3" Visibility="Collapsed" />
        <DataGridTextColumn Binding="{Binding [3]}" Header="4" />
        <DataGridTextColumn Binding="{Binding [4]}" Header="5" />
    </dg:DataGridEx.Columns>
</dg:DataGridEx>
```

**You can binding Content to DataGridEx's DataContext**

```
public MainWindow()
{
    InitializeComponent();
    DataContext = this;
}

public string myfooter => testdata.Count.ToString();

public ObservableCollection<int[]> testdata { get; set; } = new()
{
    new int[] { 1, 2, 3, 4, 5 },
    new int[] { 4, 5, 6, 7, 8 },
    new int[] { 7, 8, 9, 10, 11 },
    new int[] { 11, 12, 13, 14, 15 },
    new int[] { 14, 15, 16, 17, 18 },
    new int[] { 17, 18, 19, 20, 21 }
};
```

