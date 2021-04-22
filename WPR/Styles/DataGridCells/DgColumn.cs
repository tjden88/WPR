using System;
using System.Windows;
using System.Windows.Controls;

namespace WPR.Styles.DataGridCells
{
    public class WPRCheckBoxColumn : DataGridCheckBoxColumn
    {

        public WPRCheckBoxColumn()
        {
            var CellsThemes = new ResourceDictionary
            {
                Source =
                new Uri("/WPRControls;component/DataGridCells/DataGridCellsThemes.xaml", UriKind.RelativeOrAbsolute)
            };

            EditingElementStyle = CellsThemes["PRDataGridCheckBoxColumnEditingStyle"] as Style;
            ElementStyle = CellsThemes["PRDataGridCheckBoxColumnStyle"] as Style;
            CellStyle = CellsThemes["DataGridCheckBoxCellStyle"] as Style;
        }
        
    }

    public class DgTextColumn : DataGridTextColumn
    {
        public DgTextColumn()
        {
            var CellsThemes = new ResourceDictionary
            {
                Source =
                new Uri("/WPRControls;component/DataGridCells/DataGridCellsThemes.xaml", UriKind.RelativeOrAbsolute)
            };

            EditingElementStyle = CellsThemes["PRDataGridTextColumnEditingStyle"] as Style;
            ElementStyle = CellsThemes["PRDataGridTextColumnStyle"] as Style;

        }

        
    }

    public class DgComboBoxColumn : DataGridComboBoxColumn
    {
        public DgComboBoxColumn()
        {
            var CellsThemes = new ResourceDictionary
            {
                Source =
                new Uri("/WPRControls;component/DataGridCells/DataGridCellsThemes.xaml", UriKind.RelativeOrAbsolute)
            };

            EditingElementStyle = CellsThemes["PRDataGridComboBoxColumnEditingStyle"] as Style;
            ElementStyle = CellsThemes["PRDataGridComboBoxColumnStyle"] as Style;
            CellStyle = CellsThemes["DataGridComboBoxCellStyle"] as Style;

        }
    }
}
