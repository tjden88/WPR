using CommunityToolkit.Mvvm.ComponentModel;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM. Реализует <see cref="ObservableObject"/>
/// </summary>
public abstract class ViewModel : ObservableObject
{
    protected ViewModel()
    {
        this.InitializeAttributes();
    }
}