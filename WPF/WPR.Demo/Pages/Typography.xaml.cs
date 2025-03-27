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

        public List<StyleViewModel> TextBlockStyles;

        public Typography()
        {
            InitializeComponent();

            const string baseStyle = "WPRTextBlock";

            var resourceDictionary = Application.Current.Resources.MergedDictionaries
                .SelectMany(dict => dict.MergedDictionaries)
                .First(dict => dict.Source.ToString().Contains("TextBlocks"))
                ;





            TextBlockStyles = new();

            foreach (DictionaryEntry item in resourceDictionary)
            {
                if (item.Value is Style style && !Equals(baseStyle, item.Key))
                {
                    TextBlockStyles.Add(new StyleViewModel(item.Key.ToString(), style));
                }
            }

            ListBoxText2.ItemsSource = TextBlockStyles;
            
        }

       
    }
}
