using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPR.Demo.Models;
using WPR.Demo.ViewModels;
using WPR.Mvvm.ViewModels;

namespace WPR.Demo.Pages;

/// <summary>
/// Логика взаимодействия для Tests.xaml
/// </summary>
[ObservableObject]
public partial class Tests : Page
{
    public Tests()
    {
        InitializeComponent();
    }



    [RelayCommand]
    private void Test()
    {
        GeneratedViewModel generated;
        var person = new Person("Vasya", 32);
        generated = person;
        generated.Name = "Petya";

        Person changed;

        changed = generated;
    }
}