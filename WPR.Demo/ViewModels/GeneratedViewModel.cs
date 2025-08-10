using System.Diagnostics;
using WPR.Demo.Models;
using WPR.Mvvm.ViewModels;

namespace WPR.Demo.ViewModels;

public partial class GeneratedViewModel : ViewModel<Person>
{
    public GeneratedViewModel(Person model) : base(model)
    {
    }

    void OnNameChanged(string newValue)
    {
        Debug.WriteLine("NameChanged");
    }
}