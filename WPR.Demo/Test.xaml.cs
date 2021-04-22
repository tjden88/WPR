using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Shell32;

namespace WPR.Demo
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            List<string> items = new();

            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            if (shellAppType is not null)
            {
                object shell = Activator.CreateInstance(shellAppType);
                Folder2 f2 = (Folder2)shellAppType.InvokeMember("NameSpace",
                    System.Reflection.BindingFlags.InvokeMethod, null, shell,
                    new object[] { "shell:::{679f85cb-0220-4080-b29b-5540cc05aab6}" });

                if (f2 != null) items.AddRange(from FolderItem fi in f2.Items() where fi.IsFolder select fi.Path);
            }


            list.ItemsSource = items;
        }
    }
}
