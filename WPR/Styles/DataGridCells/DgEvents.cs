using System;
using System.Windows.Controls;

namespace WPR.Styles.DataGridCells
{
   public partial class DgEvents
    {
        private void WPRDataGridComboBoxColumnCell_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
                if (combo.IsDropDownOpen) (sender as ComboBox).FindVisualParent<DataGrid>().CommitEdit(DataGridEditingUnit.Row, false);
        }
    }
}
