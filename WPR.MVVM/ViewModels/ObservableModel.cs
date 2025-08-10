using CommunityToolkit.Mvvm.ComponentModel;

namespace WPR.Mvvm.ViewModels;

public abstract class ObservableModel<T>(T Model) : ObservableObject where T : class
{
    protected readonly T Model = Model ?? throw new ArgumentNullException(nameof(Model));

    // преобразование VM -> Model
    public static implicit operator T (ObservableModel<T> vm)
    {
        return vm.Model;
    }
}