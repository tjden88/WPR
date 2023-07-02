using System.Windows;
using System.Windows.Controls;

namespace WPR.Styles;

partial class DataGrids
{
    private static readonly ResourceDictionary _CellThemes = new()
    {
        Source = new Uri("/WPR;component/Styles/DataGridColumns.xaml", UriKind.Relative)
    };

    private DataGridCell _Currentcell;
    private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is DataGridCell cell && sender is DataGrid grd)
        {
            // Starts the Edit on the row;
            if (!cell.Equals(_Currentcell)) 
            {
                _Currentcell = cell;
                grd.BeginEdit(e);
            }
        }
    }

    private void DataGrid_Loaded(object Sender, RoutedEventArgs E)
    {
        if (Sender is DataGrid d)
        {
            foreach (var column in d.Columns)
                SetColumnStyle(column);

            d.Columns.CollectionChanged += Columns_CollectionChanged;
        }

    }

    private static void Columns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (var item in e.NewItems)
            {
                SetColumnStyle(item);
            }
    }

    private static void SetColumnStyle(object Column)
    {
        if (Column is DataGridTextColumn textColumn)
        {
            textColumn.EditingElementStyle = _CellThemes["WPRDataGridTextColumnEditingStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
            textColumn.ElementStyle = _CellThemes["WPRDataGridTextColumnStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
        }
        if (Column is DataGridCheckBoxColumn checkColumn)
        {
            checkColumn.EditingElementStyle = _CellThemes["WPRDataGridCheckBoxColumnEditingStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
            checkColumn.ElementStyle = _CellThemes["WPRDataGridCheckBoxColumnStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
            checkColumn.CellStyle = _CellThemes["WPRDataGridCheckBoxCellStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
        }
        if (Column is DataGridComboBoxColumn comboColumn)
        {
            comboColumn.EditingElementStyle = _CellThemes["WPRDataGridComboBoxColumnEditingStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
            comboColumn.ElementStyle = _CellThemes["WPRDataGridComboBoxColumnStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
            comboColumn.CellStyle = _CellThemes["WPRDataGridComboBoxCellStyle"] as Style ?? throw new ArgumentException("Стиль не найден");
        }
    }
}