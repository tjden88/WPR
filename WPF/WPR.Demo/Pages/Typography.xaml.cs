using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPR.Demo.Models;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для Typography.xaml
    /// </summary>
    public partial class Typography : Page
    {
        private const string BaseStyleName = "WPRTextBlock";

        public List<StyleViewModel> TextBlockStyles;

        public Typography()
        {
            InitializeComponent();

        }

        public void Update()
        {

            var resourceDictionary = Application.Current.Resources.MergedDictionaries
                    .SelectMany(dict => dict.MergedDictionaries)
                    .First(dict => dict.Source.ToString().Contains("TextBlocks"))
                ;


            var items = resourceDictionary
                    .Cast<DictionaryEntry>()
                    .Where(Entry => !Equals(BaseStyleName, Entry.Key) && Entry.Value is Style)
                    .Select(Entry =>
                    {
                        var style = (Style)Entry.Value;

                        var fontSizeSetter = style.Setters.Cast<Setter>().FirstOrDefault(s => s.Property.Name.Equals("FontSize"));
                        var fontSize = int.Parse(fontSizeSetter?.Value?.ToString() ?? "14");

                        return new
                        {
                            name = Entry.Key.ToString(),
                            style = (Style)Entry.Value,
                            fontSize
                        };
                    })
                    .OrderBy(item => item.fontSize)
                    .ThenBy(item => item.name)
                    .Select(item => new StyleViewModel(item.name, item.style))
                ;


            TextBlockStyles = new(items);
            ListBoxText2.ItemsSource = TextBlockStyles;
        }

        private void Typography_OnLoaded(object Sender, RoutedEventArgs E)
        {
            Update();
        }
    }
}
