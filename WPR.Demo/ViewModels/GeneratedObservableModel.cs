using System.Diagnostics;
using WPR.Demo.Models;
using WPR.Mvvm.ViewModels;

namespace WPR.Demo.ViewModels;

public partial class GeneratedObservableModel(Person Model) : ObservableModel<Person>(Model)
{
    void OnNameChanged(string newValue)
    {
        Debug.WriteLine("NameChanged");
    }
}