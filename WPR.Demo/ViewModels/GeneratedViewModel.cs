using WPR.Demo.Models;
using WPR.Mvvm.ViewModels;

namespace WPR.Demo.ViewModels;

public partial class GeneratedViewModel : ViewModel<Person>
{
    public GeneratedViewModel()
    {
        _age = 20;
    }
}