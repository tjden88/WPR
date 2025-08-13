using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM. Реализует <see cref="ObservableObject"/>
/// </summary>
[ObservableRecipient] // Todo: убрать после рефакторинга
public abstract partial class ViewModel : ObservableObject
{
    protected ViewModel()
    {
        this.InitializeAttributes();
    }
}