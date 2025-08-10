using System.Collections.ObjectModel;
using System.Diagnostics;
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
        GeneratedObservableModel generated;
        var person = new Person("Vasya", 32);
        generated = person;
        generated.PropertyChanged += (sender, args) => Debug.WriteLine("Changed: " + args.PropertyName);
        
        
        generated.Name = "Petya";
        generated.Roles = new(["Admin", "User"]);

        System.Linq.Enumerable.ToList(generated.Persons);
    }
}