using System;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Styles;

partial class DataGrids
{
    private static readonly ResourceDictionary CellThemes = new()
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
            textColumn.EditingElementStyle = (CellThemes["WPRDataGridTextColumnEditingStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
            textColumn.ElementStyle = (CellThemes["WPRDataGridTextColumnStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
        }
        if (Column is DataGridCheckBoxColumn checkColumn)
        {
            checkColumn.EditingElementStyle = (CellThemes["WPRDataGridCheckBoxColumnEditingStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
            checkColumn.ElementStyle = (CellThemes["WPRDataGridCheckBoxColumnStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
            checkColumn.CellStyle = (CellThemes["WPRDataGridCheckBoxCellStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
        }
        if (Column is DataGridComboBoxColumn comboColumn)
        {
            comboColumn.EditingElementStyle = (CellThemes["WPRDataGridComboBoxColumnEditingStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
            comboColumn.ElementStyle = (CellThemes["WPRDataGridComboBoxColumnStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
            comboColumn.CellStyle = (CellThemes["WPRDataGridComboBoxCellStyle"] as Style) ?? throw new ArgumentException("Стиль не найден");
        }
    }
}