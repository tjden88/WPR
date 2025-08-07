using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPR.Demo.Models;
using WPR.Dialogs;
using WPR.Mvvm.ViewModels;

namespace WPR.Demo;

/// <summary>
/// Логика взаимодействия для WprDialog.xaml
/// </summary>
public partial class WprDialog
{

    public WprDialog()
    {
        InitializeComponent();
    }
}


public partial class WprDialogViewModel : EditViewModel<Person>, IWPRDialog
{
    public event Action<bool> Completed;

    public object DialogContent { get; set; }

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Bind]
    [Required(ErrorMessage = "Имя надда!")]
    private string _Name;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Bind(nameof(Model.Age))]
    [Range(0, 120, ErrorMessage = "Возраст должен быть в диапазоне от 0 до 120 лет.")]
    private int _Age;

    /// <inheritdoc/>
    public WprDialogViewModel(Person model) : base(model)
    {
        
    }

    [AutoNotifyCanExecuteChanged]
    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            // Если есть ошибки валидации, не сохраняем и не закрываем диалог
            return;
        }
        CommitChanges();
       Completed?.Invoke(true);
    }
    public bool CanSave() => !HasErrors;

    [RelayCommand]
    private void Cancel()
    {
        Completed?.Invoke(false);
    }

    [AutoNotifyCanExecuteChanged]
    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        ResetChanges();
    }

    public bool CanReset() => HasChanges;
}
