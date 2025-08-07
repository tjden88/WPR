using System.Windows.Controls;
using WPR.Theme;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Hello.xaml
    /// </summary>
    public partial class Hello : Page
    {
        public Hello()
        {
            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ComboBoxFont.SelectedItem is StyleFont font ? font : StyleFont.Roboto;
            StyleHelper.SetBaseFont(item);
        }
    }
}
